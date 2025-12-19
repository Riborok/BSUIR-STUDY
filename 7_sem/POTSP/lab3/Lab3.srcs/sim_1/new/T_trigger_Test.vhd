library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity T_trigger_Test is
--  Port ( );
end T_trigger_Test;

architecture Behavioral of T_trigger_Test is
    component T_trigger is
        Port ( 
            T, CLK: in STD_LOGIC; 
            Q, nQ: out STD_LOGIC);
    end component;
    
    signal T, CLK: STD_LOGIC := '0';
    signal Q, nQ: STD_LOGIC;
    
    constant CLK_PERIOD: time := 20 ns;
begin
    uut: T_trigger
        port map ( 
            T => T, 
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
        T <= '1';
        wait for CLK_PERIOD;
        
        T <= '0';
        wait for CLK_PERIOD * 2;
        
        T <= '1';
        wait for CLK_PERIOD * 3;
        
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
