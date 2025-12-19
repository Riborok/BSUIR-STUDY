library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_trigger_Test is
--  Port ( );
end D_trigger_Test;

architecture Behavioral of D_trigger_Test is
    component D_trigger is
        Port ( 
            D, CLK: in STD_LOGIC; 
            Q, nQ: out STD_LOGIC );
    end component;
    
    signal D, CLK: STD_LOGIC := '0';
    signal Q, nQ: STD_LOGIC;
    
    constant CLK_PERIOD : time := 20 ns;
begin
    uut: D_trigger
        port map ( 
            D => D, 
            CLK => CLK, 
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
        
        D <= '1';
        wait for CLK_PERIOD;
        
        D <= '0';
        wait for CLK_PERIOD;
        
        D <= '1';
        wait for CLK_PERIOD;     
        
        D <= '0';
        wait for CLK_PERIOD;   
        
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
