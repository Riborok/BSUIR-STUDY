package by.kukhatskavolets.lab3

import kotlin.math.exp

fun applyGaussianBlur(imageArray: Array<Array<IntArray>>, boxSize: Int, sigma: Double): Array<Array<IntArray>> {
	val (height, width) = extractDimensions(imageArray)
	val pad = boxSize / 2
	val kernel = gaussianKernel(boxSize, sigma)

	val blurred = Array(height) { Array(width) { IntArray(channels) } }

	for (y in 0..<height) {
		for (x in 0..<width) {
			for (c in 0..<channels) {
				var sum = 0.0

				for (dy in -pad..<pad) {
					for (dx in -pad..<pad) {
						val ny = y + dy
						val nx = x + dx

						if (ny in 0..<height && nx in 0..<width) {
							val kVal = kernel[dy + pad][dx + pad]
							sum += imageArray[ny][nx][c] * kVal
						}
					}
				}

				blurred[y][x][c] = sum.toInt().coerceIn(0, 255)
			}
		}
	}

	return blurred
}

fun gaussianKernel(kernelSize: Int, sigma: Double): Array<DoubleArray> {
	val kernel = Array(kernelSize) { DoubleArray(kernelSize) }
	var sumVal = 0.0
	val center = kernelSize / 2

	for (i in 0..<kernelSize) {
		for (j in 0..<kernelSize) {
			val x = i - center
			val y = j - center
			kernel[i][j] = (1 / (2 * PI * sigma * sigma)) * exp(-(x * x + y * y) / (2 * sigma * sigma))
			sumVal += kernel[i][j]
		}
	}

	for (i in 0..<kernelSize) {
		for (j in 0..<kernelSize) {
			kernel[i][j] /= sumVal
		}
	}

	return kernel
}
