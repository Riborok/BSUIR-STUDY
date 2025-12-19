----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 10.11.2025 18:07:45
-- Design Name: 
-- Module Name: counter_FSM - Behavioral
-- Project Name: 
-- Target Devices: 
-- Tool Versions: 
-- Description: 
-- 
-- Dependencies: 
-- 
-- Revision:
-- Revision 0.01 - File Created
-- Additional Comments:
-- 
----------------------------------------------------------------------------------

library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

entity counter_FSM is
    port (
        CLK : in std_logic;                      -- тактовый сигнал
        RST : in std_logic;                      -- асинхронный сброс
        COUNT : out std_logic_vector(3 downto 0) -- текущее состояние
    );
end counter_FSM;

architecture Behavioral of counter_FSM is
    subtype state_type is natural range 0 to 15;            -- состояния автомата
    signal state_reg, state_next : state_type := 0;
begin
    -- Процесс состояния
    process(CLK, RST)
    begin
        if RST = '1' then
            state_reg <= 0;
        elsif rising_edge(CLK) then
            state_reg <= state_next;
        end if;
    end process;

    -- Логика переходов (конечный автомат)
    process(state_reg)
    begin
        if state_reg = 15 then
            state_next <= 0;
        else
            state_next <= state_reg + 1;
        end if;
    end process;

    COUNT <= std_logic_vector(to_unsigned(state_reg, 4));
end Behavioral;
