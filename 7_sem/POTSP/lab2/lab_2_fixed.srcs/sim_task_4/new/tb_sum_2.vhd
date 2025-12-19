----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_sum_2 - Behavioral
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

entity tb_sum_2 is
end tb_sum_2;

architecture Behavioral of tb_sum_2 is

component sum_2_beh
    Port ( A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           A2 : in STD_LOGIC;
           B2 : in STD_LOGIC;
           CI : in STD_LOGIC;
           S1 : out STD_LOGIC;
           S2 : out STD_LOGIC;
           CO : inout STD_LOGIC);
end component;

component sum_2_struct
    Port ( A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           A2 : in STD_LOGIC;
           B2 : in STD_LOGIC;
           CI : in STD_LOGIC;
           S1 : out STD_LOGIC;
           S2 : out STD_LOGIC;
           CO : inout STD_LOGIC);
end component;

signal a1: std_logic := '0';
signal b1: std_logic := '0';
signal a2: std_logic := '0';
signal b2: std_logic := '0';
signal ci: std_logic := '0';

signal s1_beh: std_logic;
signal s2_beh: std_logic;
signal co_beh: std_logic;
signal s1_struct: std_logic;
signal s2_struct: std_logic;
signal co_struct: std_logic;

signal error_s1: std_logic;
signal error_s2: std_logic;
signal error_co: std_logic;

constant period : time := 10 ns;

begin
    uut_beh: sum_2_beh PORT MAP (
        a1 => a1,
        b1 => b1,
        a2 => a2,
        b2 => b2,
        ci => ci,
        s1 => s1_beh,
        s2 => s2_beh,
        co => co_beh
    );
    
    uut_struct: sum_2_struct PORT MAP (
        a1 => a1,
        b1 => b1,
        a2 => a2,
        b2 => b2,
        ci => ci,
        s1 => s1_struct,
        s2 => s2_struct,
        co => co_struct
    );
    
    a1 <= not a1 after period;
    b1 <= not b1 after period*2;
    a2 <= not a2 after period*4;
    b2 <= not b2 after period*8;
    ci <= not ci after period*16;
    
    error_s1 <= s1_beh xor s1_struct;
    error_s2 <= s2_beh xor s2_struct;
    error_co <= co_beh xor co_struct;

end Behavioral;
