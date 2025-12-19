----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_and_5 - Behavioral
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

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity tb_and_5 is
end tb_and_5;

architecture Behavioral of tb_and_5 is

component and_5_beh
    port (
        I: in std_logic_vector (4 downto 0);
        O: out std_logic);
end component;

component and_5_struct
    port (
        I: in std_logic_vector (4 downto 0);
        O: out std_logic);
end component;

signal I: std_logic_vector (4 downto 0) := (others => '0');

signal o_beh: std_logic;
signal o_struct: std_logic;

signal error: std_logic;

constant period : time := 10 ns;

begin
    uut_beh: and_5_beh port map (
        I => I,
        O => o_beh
    );
    
    uut_struct: and_5_struct PORT MAP (
        I => I,
        O => o_struct
    );
    
    I(0) <= not I(0) after period;
    I(1) <= not I(1) after period*2;
    I(2) <= not I(2) after period*4;
    I(3) <= not I(3) after period*8;
    I(4) <= not I(4) after period*16;
    
    error <= o_beh xor o_struct;

end Behavioral;
