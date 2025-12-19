package by.kukhatskavolets.lab3

fun applyMedianBlur(imageArray: Array<Array<IntArray>>, boxSize: Int): Array<Array<IntArray>> {
	val (height, width) = extractDimensions(imageArray)
	val pad = boxSize / 2

	val blurred = Array(height) { Array(width) { IntArray(channels) } }

	for (y in 0..<height) {
		for (x in 0..<width) {
			for (c in 0..<channels) {
				val pixelValues = mutableListOf<Int>()
				var count = 0
				for (dy in -pad..<pad) {
					for (dx in -pad..<pad) {
						val ny = y + dy
						val nx = x + dx

						if (ny in 0..<height && nx in 0..<width) {
							pixelValues.add(imageArray[ny][nx][c])
							count++
						}
					}
				}
				pixelValues.sort()
				blurred[y][x][c] = if (count > 0) pixelValues[pixelValues.size / 2] else 0
			}
		}
	}

	return blurred
}