library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_trigger_async_start_Test is
--  Port ( );
end D_trigger_async_start_Test;

architecture Behavioral of D_trigger_async_start_Test is
    component D_trigger_async_start is
        Port (
            D, CLK, Pre: in STD_LOGIC;
            Q, nQ: out STD_LOGIC
        );
    end component;
    
    signal D, CLK, Pre: STD_LOGIC;
    signal Q, nQ: STD_LOGIC;
    
    constant CLK_PERIOD : time := 20 ns;
begin
    uut: D_trigger_async_start
        port map (
            D => D,
            CLK => CLK,
            PRE => Pre,
            Q => Q,
            nQ => nQ
        );
    
    proc: process
    begin
        CLK <= '0';
        wait for CLK_PERIOD / 2;
        CLK <= '1';
        wait for CLK_PERIOD / 2;
    end process;
    
    stim_proc: process
    begin
        Pre <= '0';
        wait for CLK_PERIOD;
        
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        wait for CLK_PERIOD;
        
        wait for CLK_PERIOD / 4;
        Pre <= '1';
        wait for CLK_PERIOD / 2;
        
        Pre <= '0';
        wait for CLK_PERIOD;
        
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        Pre <= '1';
        wait for CLK_PERIOD;
        
        Pre <= '0';
        wait for CLK_PERIOD;
        
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
