----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 17:44:30
-- Design Name: 
-- Module Name: sum_2_struct - Structural
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

entity sum_2_struct is
    Port ( A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           A2 : in STD_LOGIC;
           B2 : in STD_LOGIC;
           CI : in STD_LOGIC;
           S1 : out STD_LOGIC;
           S2 : out STD_LOGIC;
           CO : inout STD_LOGIC);
end sum_2_struct;

architecture Structural of sum_2_struct is

component sum_1_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           CI : in STD_LOGIC;
           S : out STD_LOGIC;
           CO : inout STD_LOGIC);
end component;

signal CO1 : STD_LOGIC;

begin

    SUM_1_1: sum_1_struct port map (A1, B1, CI, S1, CO1);
    SUM_1_2: sum_1_struct port map (A2, B2, CO1, S2, CO);

end Structural;
