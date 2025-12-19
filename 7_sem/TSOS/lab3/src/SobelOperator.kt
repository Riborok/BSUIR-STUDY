package by.kukhatskavolets.lab3

import kotlin.math.sqrt

// ядра оператора Собеля
val SOBEL_CORE: Pair<Array<IntArray>, Array<IntArray>> = Pair(
	arrayOf(intArrayOf(-1, 0, 1), intArrayOf(-2, 0, 2), intArrayOf(-1, 0, 1)),
	arrayOf(intArrayOf(1, 2, 1), intArrayOf(0, 0, 0), intArrayOf(-1, -2, -1))
)

// ядра оператора Шарра
val SHARRS_CORE: Pair<Array<IntArray>, Array<IntArray>> = Pair(
	arrayOf(intArrayOf(3, 0, -3), intArrayOf(10, 0, -10), intArrayOf(3, 0, -3)),
	arrayOf(intArrayOf(3, 10, 3), intArrayOf(0, 0, 0), intArrayOf(-3, -10, -3))
)

// ядра оператора Превитта
val PRUITTS_CORE: Pair<Array<IntArray>, Array<IntArray>> = Pair(
	arrayOf(intArrayOf(-1, 0, 1), intArrayOf(-1, 0, 1), intArrayOf(-1, 0, 1)),
	arrayOf(intArrayOf(-1, -1, -1), intArrayOf(0, 0, 0), intArrayOf(1, 1, 1))
)

// Определяем типы для ядер
typealias GradientKernel = Pair<Array<IntArray>, Array<IntArray>>

// Коллекция всех доступных ядер
val GRADIENT_KERNELS = mapOf(
	"Sobel" to SOBEL_CORE,
	"Sharr" to SHARRS_CORE,
	"Pruitt" to PRUITTS_CORE
)

// Generic функция для применения любого градиентного оператора
fun applyGradientOperator(
	imageArray: Array<Array<IntArray>>,
	kernel: GradientKernel,
	useGrayscale: Boolean = false
): Array<Array<IntArray>> {
	val (kernelX, kernelY) = kernel
	val (height, width) = extractDimensions(imageArray)
	val result = Array(height) { Array(width) { IntArray(channels) } }

	for (y in 1..<height - 1) {
		for (x in 1..<width - 1) {
			if (useGrayscale) {
				// Черно-белая версия
				var gx = 0.0
				var gy = 0.0

				for (i in -1..1) {
					for (j in -1..1) {
						val gray = (imageArray[y + i][x + j][0] +
								imageArray[y + i][x + j][1] +
								imageArray[y + i][x + j][2]) / 3
						gx += gray * kernelX[i + 1][j + 1]
						gy += gray * kernelY[i + 1][j + 1]
					}
				}

				val magnitude = sqrt(gx * gx + gy * gy).toInt().coerceIn(0..255)
				result[y][x].fill(magnitude)
			} else {
				// Цветная версия
				var gxR = 0.0; var gxG = 0.0; var gxB = 0.0
				var gyR = 0.0; var gyG = 0.0; var gyB = 0.0

				for (i in -1..1) {
					for (j in -1..1) {
						gxR += imageArray[y + i][x + j][0] * kernelX[i + 1][j + 1]
						gxG += imageArray[y + i][x + j][1] * kernelX[i + 1][j + 1]
						gxB += imageArray[y + i][x + j][2] * kernelX[i + 1][j + 1]

						gyR += imageArray[y + i][x + j][0] * kernelY[i + 1][j + 1]
						gyG += imageArray[y + i][x + j][1] * kernelY[i + 1][j + 1]
						gyB += imageArray[y + i][x + j][2] * kernelY[i + 1][j + 1]
					}
				}

				result[y][x][0] = sqrt(gxR * gxR + gyR * gyR).toInt().coerceIn(0..255)
				result[y][x][1] = sqrt(gxG * gxG + gyG * gyG).toInt().coerceIn(0..255)
				result[y][x][2] = sqrt(gxB * gxB + gyB * gyB).toInt().coerceIn(0..255)
			}
		}
	}

	return result
}

fun compareAllGradientOperators(imageArray: Array<Array<IntArray>>): Map<String, Array<Array<IntArray>>> {
	return GRADIENT_KERNELS.mapValues { (name, kernel) ->
		applyGradientOperator(imageArray, kernel, false)
	}
}

fun compareAllGradientOperatorsGray(imageArray: Array<Array<IntArray>>): Map<String, Array<Array<IntArray>>> {
	return GRADIENT_KERNELS.mapValues { (name, kernel) ->
		applyGradientOperator(imageArray, kernel, true)
	}
}
