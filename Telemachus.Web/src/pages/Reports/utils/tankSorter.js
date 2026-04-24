export default function tankSorter(x, y) {
  // Assuming x and y are objects with a property named displayOrder
  const displayOrderX = x.displayOrder
  const displayOrderY = y.displayOrder

  if (displayOrderX < displayOrderY) {
    return -1 // x should come before y
  }
  if (displayOrderX > displayOrderY) {
    return 1 // x should come after y
  }
  return 0 // x and y have the same displayOrder
}
