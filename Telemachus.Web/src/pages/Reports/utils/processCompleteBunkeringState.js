import bdnCombiner from './bdnCombiner'
import $ from './enums'

export default function processCompleteBunkeringState(tank, group, groups) {
  const summary = {}
  const totalVolume = { actual: 0, pool: 0 }
  const volumeLoaded = tank.fields.find(field => $.checkIf(field).isVolumeField())?.value
  let volume = groups
    .find(_ => $.checkIf(_).isActualGroup(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isVolumeField())?.value
  totalVolume.actual = (parseFloat(volumeLoaded || 0) + parseFloat(volume || 0)).toFixed(3)
  volume = groups
    .find(_ => $.checkIf(_).isPoolGroup(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isVolumeField())?.value
  totalVolume.pool = (parseFloat(volumeLoaded || 0) + parseFloat(volume || 0)).toFixed(3)
  summary.totalVolume = totalVolume
  summary.exceed = 0.1 * parseFloat(tank.capacity) < parseFloat(volumeLoaded || 0)
  const comminglingField = tank.fields.find(field => $.checkIf(field).isCommingleField())
  if (!comminglingField) return summary
  if (!summary.exceed) {
    comminglingField.value = '0'
  }
  if (comminglingField.value !== '1') return summary
  const bdn = {
    actual: bdnCombiner(
      groups
        .find(_ => $.checkIf(_).isActualGroup(group.name))
        ?.tanks.find(_ => _.id === tank.id)
        ?.fields.find(field => $.checkIf(field).isBdnField())?.value,
      tank.fields.find(field => $.checkIf(field).isBdnField())?.value
    ),
    pool: bdnCombiner(
      groups
        .find(_ => $.checkIf(_).isPoolGroup(group.name))
        ?.tanks.find(_ => _.id === tank.id)
        ?.fields.find(field => $.checkIf(field).isBdnField())?.value,
      tank.fields.find(field => $.checkIf(field).isBdnField())?.value
    )
  }
  summary.bdn = bdn
  let density = tank.fields.find(field => $.checkIf(field).isDensityField())?.value
  summary.density = { actual: density, pool: density }
  density = groups
    .find(_ => $.checkIf(_).isActualGroup(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isDensityField())?.value
  if (density) {
    summary.density.actual = (
      (parseFloat(summary.density.actual || 0) + parseFloat(density || 0)) /
      2
    ).toFixed(4)
  }
  density = groups
    .find(_ => $.checkIf(_).isPool(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isDensityField())?.value
  if (density) {
    summary.density.pool = (
      (parseFloat(summary.density.actual || 0) + parseFloat(density || 0)) /
      2
    ).toFixed(4)
  }
  let sulphur = tank.fields.find(field => $.checkIf(field).isSulphurContentField())?.value
  summary.sulphur = { actual: sulphur, pool: sulphur }
  sulphur = groups
    .find(_ => $.checkIf(_).isActualGroup(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isSulphurContentField())?.value
  if (sulphur) {
    summary.sulphur.actual = (
      (parseFloat(summary.sulphur.actual || 0) + parseFloat(sulphur || 0)) /
      2
    ).toFixed(4)
  }
  sulphur = groups
    .find(_ => $.checkIf(_).isPoolGroup(group.name))
    ?.tanks.find(_ => _.id === tank.id)
    ?.fields.find(field => $.checkIf(field).isSulphurContentField())?.value
  if (sulphur) {
    summary.sulphur.pool = (
      (parseFloat(summary.sulphur.pool || 0) + parseFloat(sulphur || 0)) /
      2
    ).toFixed(4)
  }
  return summary
}
