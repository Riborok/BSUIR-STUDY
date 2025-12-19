/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body (merged logic + new config)
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private typedef -----------------------------------------------------------*/
typedef struct {
    uint8_t segment;
    uint8_t mask;
} SegmentState;

/* Private define ------------------------------------------------------------*/
#define LATCH_PIN GPIO_PIN_5
#define LATCH_PORT GPIOB
#define CLOCK_PIN GPIO_PIN_8
#define CLOCK_PORT GPIOA
#define DATA_PIN  GPIO_PIN_9
#define DATA_PORT GPIOA

#define SEQ_LEN 12U
#define BASE_DELAY_MS 2000U
#define SEGMENT_ON_TIME_MS 20U
#define DEBOUNCE_DELAY_MS 250U
#define UART_RX_BUFFER_SIZE 1U

/* Private variables ---------------------------------------------------------*/
UART_HandleTypeDef huart2;

/* Snake state */
static const SegmentState sequence[SEQ_LEN] = {
    {0b00000001, 0b11111110},
    {0b00000010, 0b11111110},
    {0b00000100, 0b11111110},
    {0b00001000, 0b11111110},
    {0b00001000, 0b11111101},
    {0b00001000, 0b11111011},
    {0b00001000, 0b11110111},
    {0b00000100, 0b11110111},
    {0b00000010, 0b11110111},
    {0b00000001, 0b11110111},
    {0b00000001, 0b11101111},
    {0b00000001, 0b11011111}
};

static volatile uint8_t running = 0;
static volatile uint8_t paused = 0;
static volatile uint8_t direction_forward = 1;
static volatile uint8_t head_index = 0;
static volatile uint32_t last_interrupt_time[3] = {0, 0, 0};
static volatile uint8_t speed = 1;

static uint8_t uart_rx_buffer[UART_RX_BUFFER_SIZE];

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_USART2_UART_Init(void);

/* -------------------------------------------------------------------------- */
/*                              Shift Register                                */
/* -------------------------------------------------------------------------- */

static void transmitByte(uint8_t data) {
    for (int8_t bit = 7; bit >= 0; bit--) {
        HAL_GPIO_WritePin(DATA_PORT, DATA_PIN,
                          (data & (1U << bit)) ? GPIO_PIN_SET : GPIO_PIN_RESET);
        HAL_GPIO_WritePin(CLOCK_PORT, CLOCK_PIN, GPIO_PIN_SET);
        HAL_GPIO_WritePin(CLOCK_PORT, CLOCK_PIN, GPIO_PIN_RESET);
    }
}

static void latchFrame(const SegmentState *state) {
    HAL_GPIO_WritePin(LATCH_PORT, LATCH_PIN, GPIO_PIN_RESET);
    transmitByte(state->mask);
    transmitByte(state->segment);
    HAL_GPIO_WritePin(LATCH_PORT, LATCH_PIN, GPIO_PIN_SET);
}

static void clearDisplay(void) {
    HAL_GPIO_WritePin(LATCH_PORT, LATCH_PIN, GPIO_PIN_RESET);
    transmitByte(0xFF);
    transmitByte(0x00);
    HAL_GPIO_WritePin(LATCH_PORT, LATCH_PIN, GPIO_PIN_SET);
}

static void renderFrame(void) {
    latchFrame(&sequence[head_index]);
}

/* -------------------------------------------------------------------------- */
/*                                  Snake                                     */
/* -------------------------------------------------------------------------- */

static void advanceHeadOnce(void) {
    if (direction_forward) {
        head_index = (uint8_t)((head_index + 1U) % SEQ_LEN);
    } else {
        head_index = (head_index == 0U) ? (SEQ_LEN - 1U) : (head_index - 1U);
    }
}

static uint8_t getCurrentPosition(void) {
    return (uint8_t)(head_index + 1U);
}

static void sendPositionViaUART(void) {
    uint8_t position = getCurrentPosition();
    HAL_UART_Transmit_IT(&huart2, &position, 1);
}

static uint32_t getSpeedDelay(void) {
    return BASE_DELAY_MS / speed;
}

/* -------------------------------------------------------------------------- */
/*                                 Main                                       */
/* -------------------------------------------------------------------------- */

int main(void)
{
    HAL_Init();
    SystemClock_Config();
    MX_GPIO_Init();
    MX_USART2_UART_Init();

    clearDisplay();
    HAL_UART_Receive_IT(&huart2, uart_rx_buffer, UART_RX_BUFFER_SIZE);

    while (1)
    {
        sendPositionViaUART();

        if (running && !paused) {
            renderFrame();
            advanceHeadOnce();

            uint32_t delay = getSpeedDelay();
            if (delay > SEGMENT_ON_TIME_MS)
                HAL_Delay(delay - SEGMENT_ON_TIME_MS);
            else
                HAL_Delay(SEGMENT_ON_TIME_MS);

        } else if (running && paused) {
            renderFrame();
            HAL_Delay(80U);

        } else {
						renderFrame();
            HAL_Delay(100U);
        }
    }
}

/* -------------------------------------------------------------------------- */
/*                                  System                                    */
/* -------------------------------------------------------------------------- */

void SystemClock_Config(void)
{
    RCC_OscInitTypeDef RCC_OscInitStruct = {0};
    RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};

    RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
    RCC_OscInitStruct.HSIState = RCC_HSI_ON;
    RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
    RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;

    if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
        Error_Handler();

    RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK |
                                  RCC_CLOCKTYPE_SYSCLK |
                                  RCC_CLOCKTYPE_PCLK1 |
                                  RCC_CLOCKTYPE_PCLK2;

    RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI;
    RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
    RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
    RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

    if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_0) != HAL_OK)
        Error_Handler();
}

/* -------------------------------------------------------------------------- */
/*                             GPIO & UART Init                               */
/* -------------------------------------------------------------------------- */

static void MX_USART2_UART_Init(void)
{
    huart2.Instance = USART2;
    huart2.Init.BaudRate = 115200;
    huart2.Init.WordLength = UART_WORDLENGTH_8B;
    huart2.Init.StopBits = UART_STOPBITS_1;
    huart2.Init.Parity = UART_PARITY_NONE;
    huart2.Init.Mode = UART_MODE_TX_RX;
    huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
    huart2.Init.OverSampling = UART_OVERSAMPLING_16;

    if (HAL_UART_Init(&huart2) != HAL_OK)
        Error_Handler();
}

static void MX_GPIO_Init(void)
{
    GPIO_InitTypeDef GPIO_InitStruct = {0};

    __HAL_RCC_GPIOA_CLK_ENABLE();
    __HAL_RCC_GPIOB_CLK_ENABLE();

    /* Shift register pins */
    HAL_GPIO_WritePin(GPIOA, CLOCK_PIN | DATA_PIN, GPIO_PIN_RESET);
    HAL_GPIO_WritePin(GPIOB, LATCH_PIN, GPIO_PIN_RESET);

    GPIO_InitStruct.Pin = CLOCK_PIN | DATA_PIN;
    GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
    GPIO_InitStruct.Pull = GPIO_NOPULL;
    GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
    HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

    GPIO_InitStruct.Pin = LATCH_PIN;
    HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

    /* Buttons: PA1, PA4, PB0 */
    GPIO_InitStruct.Pin = GPIO_PIN_1 | GPIO_PIN_4;
    GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
    GPIO_InitStruct.Pull = GPIO_NOPULL;
    HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

    GPIO_InitStruct.Pin = GPIO_PIN_0;
    HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

    /* EXTI */
    HAL_NVIC_SetPriority(EXTI0_IRQn, 0, 0); // PB0
    HAL_NVIC_EnableIRQ(EXTI0_IRQn);

    HAL_NVIC_SetPriority(EXTI1_IRQn, 0, 0); // PA1
    HAL_NVIC_EnableIRQ(EXTI1_IRQn);

    HAL_NVIC_SetPriority(EXTI4_IRQn, 0, 0); // PA4
    HAL_NVIC_EnableIRQ(EXTI4_IRQn);
}

/* -------------------------------------------------------------------------- */
/*                              Interrupts                                    */
/* -------------------------------------------------------------------------- */

void HAL_GPIO_EXTI_Callback(uint16_t GPIO_Pin)
{
    uint32_t now = HAL_GetTick();
    uint8_t idx;

    if (GPIO_Pin == GPIO_PIN_1) idx = 0;
    else if (GPIO_Pin == GPIO_PIN_4) idx = 1;
    else if (GPIO_Pin == GPIO_PIN_0) idx = 2;
    else return;

    if (now - last_interrupt_time[idx] < DEBOUNCE_DELAY_MS)
        return;

    last_interrupt_time[idx] = now;

    if (GPIO_Pin == GPIO_PIN_1) {
        if (running) {
            running = 0;
            paused = 0;
            head_index = 0;
        } else {
            running = 1;
        }
    }
    else if (GPIO_Pin == GPIO_PIN_4) {
        if (running) paused = !paused;
    }
    else if (GPIO_Pin == GPIO_PIN_0) {
        direction_forward = !direction_forward;
    }
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
{
    if (huart->Instance == USART2) {
        uint8_t r = uart_rx_buffer[0];

        if (r >= '1' && r <= '5')
            speed = r - '0';
        else if (r >= 1 && r <= 5)
            speed = r;

        HAL_UART_Receive_IT(&huart2, uart_rx_buffer, UART_RX_BUFFER_SIZE);
    }
}

/* -------------------------------------------------------------------------- */

void Error_Handler(void)
{
    __disable_irq();
    while (1) {}
}