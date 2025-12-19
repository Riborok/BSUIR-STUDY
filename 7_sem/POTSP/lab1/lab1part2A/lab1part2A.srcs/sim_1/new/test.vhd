----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 06.09.2025 20:13:57
-- Design Name: 
-- Module Name: test - Behavioral
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

entity test is
end test;

architecture Behavioral of test is
    signal A, B, C, D : STD_LOGIC;
    signal Q : STD_LOGIC;

    component or_element
        Port ( A : in STD_LOGIC;
               B : in STD_LOGIC;
               C : in STD_LOGIC;
               D : in STD_LOGIC;
               Q : out STD_LOGIC);
    end component;

begin
    UUT: or_element
        Port Map ( A => A,
                   B => B,
                   C => C,
                   D => D,
                   Q => Q);

    process
        variable i_vec : std_logic_vector(3 downto 0);
    begin
        for i in 0 to 15 loop
            wait for 10 ns;
            i_vec := std_logic_vector(to_unsigned(i, 4));
            A <= i_vec(3); 
            B <= i_vec(2);
            C <= i_vec(1);
            D <= i_vec(0); 
            
        end loop;
    
        wait;
    end process;
end Behavioral;
