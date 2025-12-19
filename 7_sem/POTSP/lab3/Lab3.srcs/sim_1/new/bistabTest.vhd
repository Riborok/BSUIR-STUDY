library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity bistabTest is
--  Port ( );
end bistabTest;

architecture Behavioral of bistabTest is
    component bistab is
        Port (
            Q: out STD_LOGIC;
            nQ: out STD_LOGIC
        );
    end component;
    
    signal Q: STD_LOGIC := '0';
    signal nQ: STD_LOGIC := '1';
    constant SIM_TIME : time := 200 ns;
begin
    uut: bistab
        port map (
            Q => Q,
            nQ => nQ
        );

    stim_proc: process
    begin
        while now < SIM_TIME loop
            wait for 5 ns;
        end loop;
        report "The end" severity failure;
        wait;
    end process;
    
end Behavioral;
