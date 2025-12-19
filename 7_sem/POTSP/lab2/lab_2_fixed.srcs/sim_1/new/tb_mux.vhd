----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_mux - Behavioral
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

entity tb_mux is
end tb_mux;

architecture Behavioral of tb_mux is

component mux_beh
    port (
        A: in std_logic;
        B: in std_logic;
        S: in std_logic;
        Z: out std_logic);
end component;

component mux_struct
    port (
        A: in std_logic;
        B: in std_logic;
        S: in std_logic;
        Z: out std_logic);
end component;

signal a1: std_logic := '0';
signal b1: std_logic := '0';
signal s1: std_logic := '0';

signal z_beh: std_logic;
signal z_struct: std_logic;

signal error: std_logic;
signal test_vector: std_logic_vector (2 downto 0);

constant period : time := 10 ns;

begin
    uut_beh: mux_beh PORT MAP (
        a => a1,
        b => b1,
        s => s1,
        z => z_beh
    );
    
    uut_struct: mux_struct PORT MAP (
        a => a1,
        b => b1,
        s => s1,
        z => z_struct
    );
    
    a1 <= test_vector(0);
    b1 <= test_vector(1);
    s1 <= test_vector(2);
    
    stim_proc: process
    begin
    
        for i in 0 to 7 loop
            test_vector <= std_logic_vector(to_unsigned(i, test_vector'length));
            wait for period;
        end loop;  
        
        report "End of simulation" severity failure;
    
    end process;
    
    error <= z_beh xor z_struct;

end Behavioral;
