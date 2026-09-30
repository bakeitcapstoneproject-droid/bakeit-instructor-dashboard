import { mkdir, writeFile } from 'node:fs/promises';
import { buildSectionReport, reportColumns, reportVersion } from '../public/assets/js/domain/section-report.js';
import { rubricCriteria } from '../public/assets/js/domain/performance.js';
import { sectionReportWorkbook } from '../public/assets/js/domain/section-workbook.js';

const output = new URL('../public/assets/reports/', import.meta.url);
await mkdir(output, {recursive:true});
const section = {id:'example-section',name:'Example section'};
const options = {generatedAt:'2026-09-30T00:00:00.000Z',source:'demo'};
const report = buildSectionReport(section,[{
  id:'2026000001',name:'Example Learner',sectionId:section.id,recipe:'Cookies',sessions:2,score:88,waste:'Low',demo:true,
  assessment:{ratings:{decorum:4,kitchen_organization:4,safety_sanitation:5,baking_skills:4,product_appraisal:5},session_id:'sample-session-2',assessed_at:'2026-09-29T02:27:00.000Z',
    completion_status:'Completed',completion_percent:100,completion_scope:'Cookies recipe practice',
    safety_score_percent:80,safety_checks_passed:4,safety_checks_total:5,safety_incident_count:1,
    waste_quantity:15,waste_unit:'g',procedural_correct_steps:14,procedural_assessed_steps:16}
}, {
  id:'2026000002',name:'Example Developing Learner',sectionId:section.id,recipe:'Brownies',sessions:1,demo:true,
  assessment:{ratings:{decorum:2,kitchen_organization:3,safety_sanitation:3,baking_skills:3,product_appraisal:2}}
}, {
  id:'2026000003',name:'Example New Learner',sectionId:section.id,recipe:'Not started',sessions:0,demo:true
}],options);
await writeFile(new URL('section-report-template.xlsx',output),sectionReportWorkbook(buildSectionReport(section,[],options),{template:true}));
await writeFile(new URL('section-report-example.xlsx',output),sectionReportWorkbook(report));
await writeFile(new URL('section-report-example.json',output),JSON.stringify({report},null,2)+'\n');

const percentages = new Set(['legacy_score_percent','completion_percent','safety_score_percent','procedural_accuracy_percent']);
const counts = new Set(['session_count','safety_checks_passed','safety_checks_total','safety_incident_count','procedural_correct_steps','procedural_assessed_steps']);
const properties = Object.fromEntries(reportColumns.map(key=>[key,
  percentages.has(key) ? {type:['number','null'],minimum:0,maximum:100}
    : counts.has(key) ? {type:['integer','null'],minimum:0}
      : key==='waste_quantity' ? {type:['number','null'],minimum:0} : {type:['string','null']}
]));
for(const key of ['schema_version','generated_at','section_id','section_name','learner_id','learner_name','data_source']) properties[key]={type:'string',minLength:1};
properties.schema_version={const:reportVersion};
properties.generated_at={type:'string',format:'date-time'};
properties.assessed_at={type:['string','null'],format:'date-time'};
properties.data_source={enum:['local','demo','aws']};
properties.score_remark={enum:['Passed','Failed',null]};
for (const {key} of rubricCriteria) properties[key+'_rating']={type:['integer','null'],minimum:1,maximum:5};
properties.total_score={type:['integer','null'],minimum:5,maximum:25};
properties.max_score={const:25};
properties.completion_status={enum:['Not started','In progress','Completed','Abandoned',null]};
properties.waste_level={enum:['Low','Medium','High',null]};
const schema={
  $schema:'https://json-schema.org/draft/2020-12/schema',title:`BakeIT section report v${reportVersion}`,type:'object',
  required:['schemaVersion','generatedAt','section','rows','histories'],additionalProperties:false,
  properties:{schemaVersion:{const:reportVersion},generatedAt:{type:'string',format:'date-time'},
    section:{type:'object',required:['id','name'],additionalProperties:false,properties:{id:{type:'string',minLength:1,not:{const:'all'}},name:{type:'string',minLength:1}}},
    rows:{type:'array',items:{type:'object',required:reportColumns,additionalProperties:false,properties}},
    histories:{type:'array',items:{type:'object',additionalProperties:false,
      required:['learnerId','sectionId','sample','sessionCount','entries'],
      properties:{learnerId:{type:'string',minLength:1},sectionId:{type:'string',minLength:1},sample:{type:'boolean'},sessionCount:{type:'integer',minimum:0},
        entries:{type:'array',items:{type:'object',additionalProperties:false,
          required:['id','recipe','startedAt','endedAt','status','score','result'],properties:{
            id:{type:'string',minLength:1},recipe:{type:'string',minLength:1},
            startedAt:{type:['string','null'],format:'date-time'},endedAt:{type:['string','null'],format:'date-time'},
            status:{enum:['Completed','In progress','Abandoned']},score:{type:['integer','null'],minimum:5,maximum:25},
            result:{enum:['Passed','Failed','Awaiting assessment']}
          }}}
      }}}
  }
};
await writeFile(new URL('section-report.schema.json',output),JSON.stringify(schema,null,2)+'\n');
console.log('Excel template, fictional workbook, JSON example and report schema generated.');
