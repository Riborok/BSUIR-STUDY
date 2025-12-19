library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_trigger is
    port (
        Din, CLK, EN: in std_logic;
        Dout: out std_logic
    );
end DE_trigger;

architecture Behavioral of DE_trigger is
    signal Q_int: std_logic;
begin
    proc: process(CLK)
    begin
        if rising_edge(CLK) then
            if EN = '1' then
                Q_int <= Din;
            end if;
        end if;
    end process;
    
    Dout <= Q_int;
end Behavioral;
