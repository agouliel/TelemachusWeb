import { useMemo } from 'react'
import { groups } from 'src/context/useReports'
import PerformanceContainer from 'src/pages/Reports/components/PerformanceContainer/PerformanceContainer'
import RobContainer from 'src/pages/Reports/components/RobContainer/RobContainer'

const CollapseRow = ({
  colSpan,
  index,
  tabKey,
  fontSize,
  variant,
  requiresPlanFields,
  hidden,
  isPrinting,
  addRef
}) => {
  const groupId = useMemo(() => {
    if (!tabKey) return null
    return groups(requiresPlanFields, hidden).find(group => tabKey === group.key)?.groupId ?? tabKey
  }, [tabKey])

  return (
    <>
      {groupId && (
        <RobContainer
          fontSize={fontSize}
          addRef={addRef}
          isPrinting={isPrinting}
          variant={variant}
          index={index}
          groupId={groupId}
          colSpan={colSpan}
        />
      )}
      {typeof groupId === 'string' && (
        <PerformanceContainer
          fontSize={fontSize}
          addRef={addRef}
          isPrinting={isPrinting}
          colSpan={colSpan}
          hidden={hidden}
          variant={variant}
          index={index}
          tabKey={groupId}
        />
      )}
    </>
  )
}

export default CollapseRow
