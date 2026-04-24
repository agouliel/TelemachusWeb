import $ from './enums'

export default function processCons(groups, targetGroupId) {
  return (
    groups
      .filter(({ id }) => targetGroupId === id)
      .reduce(
        (value, { tanks }) =>
          value +
          tanks.reduce(
            (value, { fields }) =>
              value +
              fields
                .filter(field => $.checkIf(field).isVolumeField())
                .reduce((value, { value: inputValue, relatedValue }) => {
                  if (!parseFloat(inputValue) || !parseFloat(relatedValue)) return value
                  return value + (parseFloat(relatedValue) - parseFloat(inputValue))
                }, 0),
            0
          ),
        0
      ) || 0
  )
}
