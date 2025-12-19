----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 18:26:45
-- Design Name: 
-- Module Name: sum_2_generic_struct - Behavioral
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

entity sum_2_generic_struct is
    Port ( A : in STD_LOGIC_VECTOR (1 downto 0);
           B : in STD_LOGIC_VECTOR (1 downto 0);
           CI : in STD_LOGIC;
           S : out STD_LOGIC_VECTOR (1 downto 0);
           CO : out STD_LOGIC);
end sum_2_generic_struct;

architecture Behavioral of sum_2_generic_struct is

component sum_1_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           CI : in STD_LOGIC;
           S : out STD_LOGIC;
           CO : inout STD_LOGIC);
end component;

signal C : STD_LOGIC_VECTOR (2 downto 0);

begin

    C(0) <= CI;

    gen: for i in 0 to 1 generate
        sum_1: sum_1_struct port map (
            A => A(i), 
            B => B(i), 
            CI => C(i),
            S => S(i), 
            CO => C(i+1)
        );
    end generate;
    
    CO <= C(2);

end Behavioral;
