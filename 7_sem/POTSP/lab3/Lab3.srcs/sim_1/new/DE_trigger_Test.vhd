library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_trigger_Test is
--  Port ( );
end DE_trigger_Test;

architecture Behavioral of DE_trigger_Test is
    component DE_trigger is
        Port ( 
            D, CLK, CE: in STD_LOGIC; 
            Q, nQ: out STD_LOGIC);
    end component;
    
    signal D, CLK, CE: STD_LOGIC;
    signal Q, nQ: STD_LOGIC;
    
    constant CLK_PERIOD : time := 20 ns;
begin
    uut: DE_trigger
        port map ( 
            D => D, 
            CLK => CLK, 
            CE => CE, 
            Q => Q, 
            nQ => nQ);
    
    proc: process
    begin
        CLK <= '0';
        wait for CLK_PERIOD / 2;
        CLK <= '1';
        wait for CLK_PERIOD / 2;
    end process;
    
    stim_proc: process
    begin
        wait for CLK_PERIOD;
        
        CE <= '1';
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        wait for CLK_PERIOD; 
        
        CE <= '0';
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        wait for CLK_PERIOD;
        
        CE <= '1';
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        wait for CLK_PERIOD;
        
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
