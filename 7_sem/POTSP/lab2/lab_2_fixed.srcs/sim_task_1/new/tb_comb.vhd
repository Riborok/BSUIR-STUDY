----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_comb - Behavioral
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

entity tb_comb is
end tb_comb;

architecture Behavioral of tb_comb is

component comb_beh
    port (
        X: in std_logic;
        Y: in std_logic;
        Z: in std_logic;
        F: out std_logic);
end component;

component comb_struct
    port (
        X: in std_logic;
        Y: in std_logic;
        Z: in std_logic;
        F: out std_logic);
end component;

signal x: std_logic := '0';
signal y: std_logic := '0';
signal z: std_logic := '0';

signal f_beh: std_logic;
signal f_struct: std_logic;

signal error: std_logic;
signal test_vector: std_logic_vector (2 downto 0);

constant period : time := 10 ns;

begin
    uut_beh: comb_beh PORT MAP (
        x => x,
        y => y,
        z => z,
        f => f_beh
    );
    
    uut_struct: comb_struct PORT MAP (
        x => x,
        y => y,
        z => z,
        f => f_struct
    );
    
    x <= test_vector(0);
    y <= test_vector(1);
    z <= test_vector(2);
    
    stim_proc: process
    begin
        
        for i in 0 to 7 loop
            test_vector <= std_logic_vector(to_unsigned(i, test_vector'length));
            wait for period;
        end loop;  
        
        report "End of simulation" severity failure;
  
    end process;
    
    error <= f_beh xor f_struct;

end Behavioral;
