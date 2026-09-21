using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class MeasuringScoop : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Optional child object used to show ingredient inside the scoop.")]
    private GameObject contentsVisual;

    [SerializeField]
    [Tooltip("Child chip cluster shown instead of powder when carrying chocolate chips.")]
    private GameObject chocolateChipContentsVisual;

    [SerializeField]
    [Tooltip("This exact measuring vessel may also carry chocolate chips.")]
    private bool acceptsChocolateChips;

    [SerializeField]
    [Tooltip("Exact recipe measurement represented by this tool. Whole Item keeps legacy scene behaviour.")]
    private RecipeMeasureTool measureTool;

    [SerializeField] private string toolDisplayName;
    [SerializeField] private string measureUnit;

    [SerializeField]
    [Tooltip("Existing collider-free chip cluster used to fill the small cup.")]
    private GameObject chocolateChipContentsPrefab;

    [SerializeField, Min(0.001f)] private float cupInnerRadius = 0.0205f;
    [SerializeField] private float cupFillBottom = 0.005f;
    [SerializeField] private float cupFillSurface = 0.023f;
    [SerializeField, Min(0f)] private float chipMoundHeight = 0.005f;

    [SerializeField]
    [Tooltip("Shown for testing. Leave empty when the scoop starts clean.")]
    private string currentIngredient;

    [SerializeField]
    [Tooltip("Color used by the ingredient visual inside the scoop.")]
    private Color contentsColor = new Color(0.98f, 0.97f, 0.93f, 1f);

    [SerializeField]
    [Tooltip("Slightly lighter color used when the scoop contains sugar.")]
    private Color sugarContentsColor = new Color(1f, 0.985f, 0.94f, 1f);

    [SerializeField, Min(.005f)]
    [Tooltip("Extra aim forgiveness around the cup opening when collecting a measured ingredient.")]
    private float scoopAssistRadius = .045f;

    private GameObject[] chocolateChipContentsPile;
    [SerializeField] private Mesh[] walnutKernelMeshes;
    [SerializeField] private Material walnutKernelMaterial;
    private GameObject walnutContentsPile;
    private Material powderMaterial;
    private float nextWrongToolFeedbackTime;
    private PickupController pickup;
    private Rigidbody body;
    private MeasuredIngredientSource blockedReturnSource;
    private Component previousDestination;
    private RecipeDefinition activeRecipe;
    private float pourTime, discardTime, nextPourAttempt;
    [SerializeField, Range(45f,120f)] private float pourAngle = 75f;
    [SerializeField, Min(.1f)] private float pourHoldSeconds = .35f;
    [SerializeField, Min(.2f)] private float discardHoldSeconds = .60f;

    public static event System.Action<MeasureTransfer> ContentsTransferred;
    public RecipeMeasureTool MeasureTool => measureTool != RecipeMeasureTool.WholeItem
        ? measureTool
        : acceptsChocolateChips
            ? RecipeMeasureTool.SmallMeasuringSpoon
            : RecipeMeasureTool.MeasuringCup;
    public string ToolName => !string.IsNullOrWhiteSpace(toolDisplayName)
        ? toolDisplayName.Trim()
        : GetDefaultToolName(MeasureTool);
    public string MeasureUnit => !string.IsNullOrWhiteSpace(measureUnit)
        ? measureUnit.Trim()
        : GetDefaultMeasureUnit(MeasureTool);
    public bool IsHeld => pickup != null && pickup.HeldObject == body;
    public bool IsTilted => Vector3.Angle(transform.up, Vector3.up) >= pourAngle;
    public Vector3 MouthPosition => transform.TransformPoint(new Vector3(0,cupFillSurface,0));
    public Vector3 PourOrigin => MouthPosition + Vector3.ProjectOnPlane(Vector3.down,transform.up).normalized * cupInnerRadius;
    public float DiscardProgress => Mathf.Clamp01(discardTime/discardHoldSeconds);

    public bool AcceptsChocolateChips => acceptsChocolateChips;

    public bool IsFilled =>
        !string.IsNullOrWhiteSpace(currentIngredient);

    public string CurrentIngredient => currentIngredient;

    private void Awake()
    {
        pickup=FindAnyObjectByType<PickupController>();body=GetComponent<Rigidbody>();
        activeRecipe = FindAnyObjectByType<CookieRecipeSessionController>()?.ActiveRecipe ?? FindAnyObjectByType<BowlReceiver>()?.ActiveRecipe;
        CreateFallbackPowderVisual();
        CreateChocolateChipContentsPile();
        CreateWalnutContentsPile();
        UpdateContentsVisual();
    }

    private void Update()
    {
        if(blockedReturnSource!=null && Vector3.Distance(MouthPosition,blockedReturnSource.GetComponent<Collider>().ClosestPoint(MouthPosition))>.07f)
            blockedReturnSource=null;
        if(IsHeld && !IsFilled) TryFillFromNearbySource();
        if(!IsHeld || !IsFilled){pourTime=0;discardTime=0;previousDestination=null;return;}
        if(Keyboard.current!=null && Keyboard.current.qKey.isPressed)
        {
            pourTime=0;previousDestination=null;discardTime+=Time.deltaTime;
            if(discardTime>=discardHoldSeconds)DiscardContents();
            return;
        }
        discardTime=0;
        if(!IsTilted){pourTime=0;previousDestination=null;return;}
        Component destination=FindPourDestination();
        if(destination==null){pourTime=0;previousDestination=null;return;}
        if(destination!=previousDestination){previousDestination=destination;pourTime=0;}
        pourTime+=Time.deltaTime;
        if(pourTime<pourHoldSeconds || Time.unscaledTime<nextPourAttempt)return;
        nextPourAttempt=Time.unscaledTime+2f;pourTime=0;
        if(destination is BowlReceiver bowl) bowl.TryPourMeasure(this);
        else if(destination is PourableIngredientReceiver batterBowl) batterBowl.TryPourMeasure(this);
        else if(destination is MeasuredIngredientSource source) source.TryReturnMeasure(this);
    }

    private void TryFillFromNearbySource()
    {
        MeasuredIngredientSource nearestSource = null;
        float nearestDistance = float.PositiveInfinity;

        foreach (Collider candidate in Physics.OverlapSphere(
                     MouthPosition,
                     scoopAssistRadius,
                     Physics.DefaultRaycastLayers,
                     QueryTriggerInteraction.Collide))
        {
            MeasuredIngredientSource source =
                candidate.GetComponentInParent<MeasuredIngredientSource>();
            if (source == null || !CanFillFrom(source) ||
                !CanMeasure(source.IngredientName))
                continue;

            float distance = Vector3.Distance(
                MouthPosition,
                candidate.ClosestPoint(MouthPosition));
            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearestSource = source;
        }

        if (nearestSource != null)
            TryFill(nearestSource.IngredientName);
    }

    public Component FindPourDestination()
    {
        Vector3 origin=PourOrigin;
        // Also support a mouth already inside the tall interaction trigger.
        foreach(var c in Physics.OverlapSphere(origin,.008f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Collide))
        {
            if(c.attachedRigidbody==body)continue;
            if(c.TryGetComponent<BowlReceiver>(out var bowl))return bowl;
            if(c.TryGetComponent<PourableIngredientReceiver>(out var batterBowl))return batterBowl;
            if(c.TryGetComponent<MeasuredIngredientSource>(out var source))return source;
        }
        var hits=Physics.RaycastAll(origin+Vector3.up*.01f,Vector3.down,.35f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Collide);
        System.Array.Sort(hits,(a,b)=>a.distance.CompareTo(b.distance));
        foreach(var hit in hits)
        {
            if(hit.rigidbody==body || (pickup!=null && hit.transform.IsChildOf(pickup.transform)))continue;
            if(hit.collider.TryGetComponent<BowlReceiver>(out var bowl))return bowl;
            if(hit.collider.TryGetComponent<PourableIngredientReceiver>(out var batterBowl))return batterBowl;
            if(hit.collider.TryGetComponent<MeasuredIngredientSource>(out var source))return source;
            if(!hit.collider.isTrigger)return null;
        }
        return null;
    }

    public bool CanFillFrom(MeasuredIngredientSource source) => IsHeld && blockedReturnSource!=source &&
        Vector3.Angle(transform.up,Vector3.up)<55f && (Keyboard.current==null || !Keyboard.current.qKey.isPressed);

    public bool CanMeasure(string ingredient)
    {
        if (activeRecipe != null)
        {
            return activeRecipe.TryGetIngredientForTool(
                ingredient,
                MeasureTool,
                out _);
        }

        return MeasureTool switch
        {
            RecipeMeasureTool.OneCup or RecipeMeasureTool.MeasuringCup =>
                string.Equals(ingredient, "Flour", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ingredient, "Sugar", System.StringComparison.OrdinalIgnoreCase),
            RecipeMeasureTool.OneTeaspoon =>
                string.Equals(ingredient, "Vanilla", System.StringComparison.OrdinalIgnoreCase),
            RecipeMeasureTool.HalfTeaspoon =>
                string.Equals(ingredient, "BakingSoda", System.StringComparison.OrdinalIgnoreCase),
            RecipeMeasureTool.QuarterTeaspoon =>
                string.Equals(ingredient, "Salt", System.StringComparison.OrdinalIgnoreCase),
            RecipeMeasureTool.HalfCup when acceptsChocolateChips =>
                string.Equals(ingredient, "ChocolateChips", System.StringComparison.OrdinalIgnoreCase),
            RecipeMeasureTool.ChocolateChipScoop or RecipeMeasureTool.SmallMeasuringSpoon =>
                string.Equals(ingredient, "ChocolateChips", System.StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    public void SetActiveRecipe(RecipeDefinition recipe)
    {
        activeRecipe = recipe;
    }

    public bool DiscardContents()
    {
        if(!IsHeld || !IsFilled)return false;
        string ingredient=currentIngredient;
        var source=FindPourDestination() as MeasuredIngredientSource;
        if(source!=null)blockedReturnSource=source;
        bool discarded=CompleteTransfer(MeasureDisposition.Discarded,"Discard",PourOrigin+Vector3.down*.25f);
        if(discarded)RecipeFeedback.Warning($"Discarded 1 {MeasureUnit} of {ingredient}. Recorded as waste.");
        return discarded;
    }

    public bool CompleteTransfer(MeasureDisposition disposition,string destination,Vector3 visualTarget)
    {
        if(!TryEmpty(out var ingredient))return false;
        pourTime=0;discardTime=0;previousDestination=null;nextPourAttempt=0;
        MeasureTransferVisual.Show(PourOrigin,visualTarget,ingredient, ingredient == "Walnuts" ? walnutKernelMeshes : null);
        ContentsTransferred?.Invoke(new MeasureTransfer(ingredient,ToolName,MeasureUnit,destination,disposition));
        return true;
    }

    public void BlockRefillUntilLifted(MeasuredIngredientSource source) => blockedReturnSource=source;

    private void CreateFallbackPowderVisual()
    {
        if (contentsVisual == null)
        {
            contentsVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            contentsVisual.name = "PowderContents (Runtime)";
            contentsVisual.transform.SetParent(transform, false);
            contentsVisual.transform.localPosition =
                new Vector3(0f, (cupFillBottom + cupFillSurface) * 0.5f, 0f);
            contentsVisual.transform.localScale =
                new Vector3(cupInnerRadius * 2f,
                    (cupFillSurface - cupFillBottom) * 0.5f,
                    cupInnerRadius * 2f);
            RemoveRuntimeCollider(contentsVisual);
        }
    }

    private static void RemoveRuntimeCollider(GameObject visual)
    {
        Collider visualCollider = visual.GetComponent<Collider>();

        if (visualCollider == null)
            return;

        visualCollider.enabled = false;
        Destroy(visualCollider);
    }

    private void CreateChocolateChipContentsPile()
    {
        // Old scene fills stay hidden, including the large spoon's chip child.
        if (chocolateChipContentsVisual != null)
            chocolateChipContentsVisual.SetActive(false);

        if (!acceptsChocolateChips || chocolateChipContentsPrefab == null)
            return;

        MeshFilter filter = chocolateChipContentsPrefab.GetComponent<MeshFilter>();
        if (filter == null || filter.sharedMesh == null)
            return;

        Mesh mesh = filter.sharedMesh;
        Bounds bounds = mesh.bounds;
        float meshRadius = new Vector2(bounds.extents.x, bounds.extents.z).magnitude;
        if (mesh.isReadable)
        {
            meshRadius = 0f;
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 offset = vertex - bounds.center;
                meshRadius = Mathf.Max(meshRadius, new Vector2(offset.x, offset.z).magnitude);
            }
        }

        float fillDepth = Mathf.Max(0.001f, cupFillSurface - cupFillBottom);
        float layerDepth = fillDepth * 0.5f;
        Vector3 scale = new Vector3(
            cupInnerRadius / Mathf.Max(0.001f, meshRadius),
            layerDepth / Mathf.Max(0.001f, bounds.size.y),
            cupInnerRadius / Mathf.Max(0.001f, meshRadius));
        // Two interleaved clusters at each height close the visible gaps.
        const int heightCount = 4;
        chocolateChipContentsPile = new GameObject[heightCount * 2];

        for (int layerIndex = 0; layerIndex < chocolateChipContentsPile.Length; layerIndex++)
        {
            GameObject layer = Instantiate(chocolateChipContentsPrefab, transform);
            layer.name = $"ChocolateChipContentsLayer{layerIndex + 1} (Runtime)";
            int heightIndex = layerIndex / 2;
            Quaternion rotation = Quaternion.Euler(
                0f, heightIndex * 67f + (layerIndex % 2) * 33f, 0f);
            float height = cupFillBottom + chipMoundHeight + layerDepth * 0.5f +
                (fillDepth - layerDepth) * heightIndex / (heightCount - 1);
            layer.transform.localScale = scale;
            layer.transform.localRotation = rotation;
            // The shared cluster's mesh pivot is below its visible chips.
            // Center the mesh itself inside the cup, with a shallow mound.
            layer.transform.localPosition = new Vector3(0f, height, 0f) -
                rotation * Vector3.Scale(bounds.center, scale);
            chocolateChipContentsPile[layerIndex] = layer;
        }
    }

    public bool TryFill(string ingredientName)
    {
        if (string.IsNullOrWhiteSpace(ingredientName))
            return false;

        string nextIngredient = ingredientName.Trim().ToLowerInvariant() switch
        {
            "flour" => "Flour",
            "sugar" => "Sugar",
            "vanilla" => "Vanilla",
            "vanillaextract" => "Vanilla",
            "bakingsoda" => "BakingSoda",
            "salt" => "Salt",
            "chocolatechips" => "ChocolateChips",
            "walnuts" => "Walnuts",
            _ => ingredientName.Trim()
        };

        if (activeRecipe != null &&
            activeRecipe.TryGetIngredient(nextIngredient, out RecipeIngredientRequirement requirement))
        {
            nextIngredient = requirement.IngredientId;
        }

        if(!CanMeasure(nextIngredient))
        {
            if(Time.unscaledTime>=nextWrongToolFeedbackTime)
            {
                if (activeRecipe != null &&
                    activeRecipe.TryGetIngredient(nextIngredient, out RecipeIngredientRequirement wrongToolRequirement))
                {
                    RecipeFeedback.Warning(
                        $"Use {activeRecipe.GetToolDisplayName(wrongToolRequirement.RequiredTool)} for {wrongToolRequirement.DisplayName}.");
                }
                else
                {
                    RecipeFeedback.Warning(activeRecipe != null
                        ? $"{nextIngredient} is not part of the active recipe."
                        : nextIngredient=="ChocolateChips"?"Use Small Measuring Spoon for chocolate chips.":"Use Measuring Cup for flour and sugar.");
                }
                nextWrongToolFeedbackTime=Time.unscaledTime+4.5f;
            }
            return false;
        }

        if(IsFilled)
        {
            if(!string.Equals(currentIngredient,nextIngredient,System.StringComparison.OrdinalIgnoreCase) && Time.unscaledTime>=nextWrongToolFeedbackTime)
            {
                RecipeFeedback.Warning($"{ToolName} still contains {currentIngredient}. Tilt over its matching source to return, or hold Q to discard.");
                nextWrongToolFeedbackTime=Time.unscaledTime+4.5f;
            }
            return false;
        }

        currentIngredient = nextIngredient;
        UpdateContentsVisual();
        RecipeFeedback.Report($"{ToolName} filled with {currentIngredient}.");
        return true;
    }

    public bool TryEmpty(out string ingredientName)
    {
        ingredientName = currentIngredient;

        if (!IsFilled)
            return false;

        currentIngredient = string.Empty;
        UpdateContentsVisual();
        return true;
    }

    private void UpdateContentsVisual()
    {
        bool walnuts = IsFilled && string.Equals(currentIngredient, "Walnuts", System.StringComparison.OrdinalIgnoreCase);
        if(walnutContentsPile) walnutContentsPile.SetActive(walnuts);
        bool containsChocolateChips = acceptsChocolateChips && IsFilled && string.Equals(
            currentIngredient?.Trim(),
            "ChocolateChips",
            System.StringComparison.OrdinalIgnoreCase);

        if (chocolateChipContentsPile != null)
        {
            foreach (GameObject chipLayer in chocolateChipContentsPile)
            {
                if (chipLayer != null)
                    chipLayer.SetActive(containsChocolateChips);
            }
        }

        if (contentsVisual != null)
            contentsVisual.SetActive(IsFilled && !containsChocolateChips && !walnuts);

        if (!IsFilled || containsChocolateChips || walnuts || contentsVisual == null)
            return;

        foreach (Renderer contentsRenderer in
                 contentsVisual.GetComponentsInChildren<Renderer>(true))
        {
            if (!powderMaterial)
                powderMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            contentsRenderer.sharedMaterial = powderMaterial;
            Material contentsMaterial = powderMaterial;
            Color displayColor = currentIngredient?.Trim().ToLowerInvariant() switch
            {
                "sugar" => sugarContentsColor,
                "vanilla" => new Color(0.30f, 0.08f, 0.025f, 1f),
                "milk" => new Color(.98f,.97f,.91f),
                "oil" => new Color(.96f,.72f,.16f),
                "bakingsoda" => new Color(0.90f, 0.92f, 0.88f, 1f),
                "bakingpowder" => new Color(0.93f, 0.94f, 0.90f, 1f),
                "cocoa" => new Color(0.20f, 0.055f, 0.015f, 1f),
                "walnuts" => new Color(0.56f, 0.32f, 0.14f, 1f),
                "salt" => new Color(0.99f, 0.98f, 0.94f, 1f),
                _ => contentsColor
            };

            if (contentsMaterial.HasProperty("_BaseColor"))
                contentsMaterial.SetColor("_BaseColor", displayColor);
            else if (contentsMaterial.HasProperty("_Color"))
                contentsMaterial.SetColor("_Color", displayColor);

            if (contentsMaterial.HasProperty("_Metallic"))
                contentsMaterial.SetFloat("_Metallic", 0f);

            if (contentsMaterial.HasProperty("_Smoothness"))
                contentsMaterial.SetFloat("_Smoothness", 0.15f);
        }
    }

    private void CreateWalnutContentsPile()
    {
        if(walnutKernelMeshes == null || walnutKernelMeshes.Length == 0 || !walnutKernelMaterial) return;
        walnutContentsPile = new GameObject("Walnut pieces in measure");
        walnutContentsPile.transform.SetParent(transform,false);
        float depth=Mathf.Max(.003f,cupFillSurface-cupFillBottom);
        const int count=72;
        for(int i=0;i<count;i++)
        {
            Mesh mesh=walnutKernelMeshes[i%walnutKernelMeshes.Length]; if(!mesh) continue;
            GameObject piece=new("Walnut chunk",typeof(MeshFilter),typeof(MeshRenderer));
            piece.transform.SetParent(walnutContentsPile.transform,false);
            piece.GetComponent<MeshFilter>().sharedMesh=mesh;
            piece.GetComponent<Renderer>().sharedMaterial=walnutKernelMaterial;
            float width=cupInnerRadius*(.34f+(i%4)*.025f);
            float scale=width/Mathf.Max(mesh.bounds.size.x,mesh.bounds.size.z);
            Quaternion rotation=Quaternion.Euler(0,i*137.5f,0);
            piece.transform.localRotation=rotation;
            piece.transform.localScale=Vector3.one*scale;
            int level=i/24;
            float radius=(cupInnerRadius-width*.58f)*Mathf.Sqrt((i%24+.5f)/24f);
            float a=i*2.399963f;
            float height=Mathf.Lerp(cupFillBottom+depth*.30f,cupFillSurface+chipMoundHeight,level/2f);
            piece.transform.localPosition=new Vector3(Mathf.Cos(a)*radius,height,Mathf.Sin(a)*radius)
                - rotation*(mesh.bounds.center*scale);
        }
        walnutContentsPile.SetActive(false);
    }

    private void OnDestroy() { if(powderMaterial) Destroy(powderMaterial); }

    private static string GetDefaultToolName(RecipeMeasureTool tool)
    {
        return tool switch
        {
            RecipeMeasureTool.OneCup => "1 Cup Measuring Cup",
            RecipeMeasureTool.OneTablespoon => "1 Tablespoon",
            RecipeMeasureTool.HalfCup => "1/2 Cup Measuring Cup",
            RecipeMeasureTool.QuarterCup => "1/4 Cup Measuring Cup",
            RecipeMeasureTool.OneTeaspoon => "1 Teaspoon",
            RecipeMeasureTool.HalfTeaspoon => "1/2 Teaspoon",
            RecipeMeasureTool.QuarterTeaspoon => "1/4 Teaspoon",
            RecipeMeasureTool.ChocolateChipScoop => "Chocolate Chip Scoop",
            RecipeMeasureTool.SmallMeasuringSpoon => "Small Measuring Spoon",
            _ => "Measuring Cup"
        };
    }

    private static string GetDefaultMeasureUnit(RecipeMeasureTool tool)
    {
        return tool switch
        {
            RecipeMeasureTool.OneCup => "1 cup",
            RecipeMeasureTool.OneTablespoon => "1 tablespoon",
            RecipeMeasureTool.HalfCup => "1/2 cup",
            RecipeMeasureTool.QuarterCup => "1/4 cup",
            RecipeMeasureTool.OneTeaspoon => "1 teaspoon",
            RecipeMeasureTool.HalfTeaspoon => "1/2 teaspoon",
            RecipeMeasureTool.QuarterTeaspoon => "1/4 teaspoon",
            RecipeMeasureTool.ChocolateChipScoop => "small scoop",
            RecipeMeasureTool.SmallMeasuringSpoon => "small spoon",
            _ => "cup measure"
        };
    }
}
