library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_Test is
--  Port ( );
end RS_Test;

architecture Behavioral of RS_Test is
    component RS_beh is
       Port (
           S, R: in STD_LOGIC;
           Q, nQ: out STD_LOGIC
       );
    end component;
    
    component RS_struct is
       Port (
           S, R: in STD_LOGIC;
           Q, nQ: out STD_LOGIC
       );
    end component;
    
    signal S: STD_LOGIC := '0';
    signal R: STD_LOGIC := '0';
    
    signal Q_beh, nQ_beh: STD_LOGIC;
    signal Q_struct, nQ_struct: STD_LOGIC;
    
    signal Error: STD_LOGIC;
begin
    uut_beh: RS_beh
        port map (
           S  => S,
           R  => R,
           Q  => Q_beh,
           nQ => nQ_beh
        );
       
    uut_struct: RS_struct
        port map (
          S  => S,
          R  => R,
          Q  => Q_struct,
          nQ => nQ_struct
        );
    
    Error <= '1' when (Q_beh /= Q_struct) or
                      (nQ_beh /= nQ_struct) else '0';
    stim_proc: process
    begin
        S <= '1'; R <= '0';
        wait for 20 ns;
        
        S <= '0'; R <= '1';
        wait for 20 ns;
        
        S <= '0'; R <= '0';
        wait for 20 ns;
        
        S <= '1'; R <= '1';
        wait for 20 ns;
        
        S <= '0'; R <= '0';
        wait for 20 ns;
        
        if Error = '1' then
            report "Error occured!" severity failure;
        end if;
        
        wait;
    end process;
end Behavioral;
