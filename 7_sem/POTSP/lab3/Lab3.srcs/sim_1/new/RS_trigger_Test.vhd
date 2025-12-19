library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_trigger_Test is
--  Port ( );
end RS_trigger_Test;

architecture Behavioral of RS_trigger_Test is
  component RS_trigger is
      Port ( 
        S, R, CLK: in STD_LOGIC; 
        Q, nQ: out STD_LOGIC);
  end component;
  
  signal S, R, CLK: STD_LOGIC;
  signal Q, nQ: STD_LOGIC;
  
  constant CLK_PERIOD: time := 20 ns;
begin
    uut: RS_trigger
        port map ( 
            S => S, 
            R => R, 
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
        S <= '0';
        R <= '0';
        wait for CLK_PERIOD;
        
        S <= '1';
        R <= '0';
        wait for CLK_PERIOD;
        
        S <= '0';
        R <= '0';
        wait for CLK_PERIOD;
                    
        S <= '0';
        R <= '1';
        wait for CLK_PERIOD;
        
        S <= '0';
        R <= '0';
        wait for CLK_PERIOD;
        
        S <= '1';
        R <= '1';
        wait for CLK_PERIOD;
                        
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
