----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 19:27:50
-- Design Name: 
-- Module Name: FDCE_trigger - Behavioral
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

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity FDCE_trigger is
    port (
        D   : in std_logic;
        CE  : in std_logic;
        CLR : in std_logic;
        CLK : in std_logic;
        Q   : out std_logic
    );  
end FDCE_trigger;

architecture Behavioral of FDCE_trigger is
    signal Q_int : std_logic := '0';
begin
    process(CLK, CLR)
    begin
        if CLR = '1' then
            Q_int <= '0';
        elsif rising_edge(CLK) then
            if CE = '1' then
                Q_int <= D;
            end if;
        end if;
    end process;

    Q <= Q_int;
end Behavioral;
