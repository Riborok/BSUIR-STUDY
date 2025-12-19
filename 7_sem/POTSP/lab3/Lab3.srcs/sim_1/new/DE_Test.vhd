library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_Test is
--  Port ( );
end DE_Test;

architecture Behavioral of DE_Test is
    component DE_beh is
        Port (
            D, E: in STD_LOGIC;
            Q, nQ: out STD_LOGIC
        );
    end component;
    
    component DE_struct is
        Port (
            D, E: in STD_LOGIC;
            Q, nQ: out STD_LOGIC
        );
    end component;
    
    signal D, E: STD_LOGIC;
    signal Q_beh, nQ_beh: STD_LOGIC;
    signal Q_struct, nQ_struct: STD_LOGIC; 
    
    signal Error: STD_LOGIC;
begin
    uut_beh: DE_beh
    port map (
       D => D,
       E => E,
       Q  => Q_beh,
       nQ => nQ_beh
    );
   
    uut_struct: DE_struct
    port map (
      D => D,
      E => E,
      Q  => Q_struct,
      nQ => nQ_struct
    );

    Error <= '1' when (Q_beh /= Q_struct) or
                      (nQ_beh /= nQ_struct) else '0';
    
    stim_proc: process
    begin
        wait for 10 ns;
        
        E <= '1';
        wait for 5 ns;
        
        D <= '1';
        wait for 20 ns;
        
        D <= '0';
        wait for 20 ns;
        
        D <= '1';
        wait for 10 ns;
        
        E <= '0';
        wait for 10 ns;
        
        D <= '0';
        wait for 20 ns;
        
        D <= '1';
        wait for 20 ns;
        
        D <= '0'; 
        E <= '1';
        wait for 10 ns;
        
        E <= '0';
        wait for 10 ns;
        
        D <= '1';
        wait for 20 ns;
        
        if Error = '1' then
            report "Error occured!" severity failure;
        end if;
        
        wait;
    end process;

end Behavioral;
