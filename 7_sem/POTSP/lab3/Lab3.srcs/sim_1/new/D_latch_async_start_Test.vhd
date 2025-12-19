library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_latch_async_start_Test is
--  Port ( );
end D_latch_async_start_Test;

architecture Behavioral of D_latch_async_start_Test is
    component D_latch_async_start is
        Port (D, E, Pre: in STD_LOGIC; Q, nQ: out STD_LOGIC);
    end component;
    
    signal D, E, Pre: STD_LOGIC := '0';
    signal Q, nQ: STD_LOGIC;
begin
    uut: D_latch_async_start
        port map (D => D, E => E, Pre => Pre, Q => Q, nQ => nQ); 
        
    stim_proc: process
    begin
        wait for 10 ns;
        
        Pre <= '0';
        E <= '1';
        D <= '1';
        wait for 20 ns;
        
        E <= '0';
        D <= '0';
        wait for 20 ns;
        
        E <= '1';
        D <= '0';
        wait for 10 ns;
        
        E <= '0';
        wait for 5 ns;
        
        Pre <= '1';
        wait for 20 ns;
        
        Pre <= '0';
        wait for 20 ns;
        
        E <= '1';
        D <= '0';
        wait for 20 ns;
        
        Pre <= '1';
        wait for 20 ns;
        
        Pre <= '0';
        wait for 20 ns;
        
        report "The end" severity failure;
        wait;
    end process;

end Behavioral;
