import $ from './enums'
import { validateField } from './validators'

export default function hasInvalidSurvey(group) {
  return (
    !!group &&
    $.checkIf(group).isRobSurveyGroup() &&
    group.tanks?.some(tank => tank.fields.some(field => validateField(field) === false))
  )
}
