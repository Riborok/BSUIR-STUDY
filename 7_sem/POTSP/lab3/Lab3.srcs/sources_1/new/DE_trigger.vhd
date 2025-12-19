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
    Port (
        D, CLK, CE: in STD_LOGIC;
        Q, nQ: out STD_LOGIC
    );
end DE_trigger;

architecture Behavioral of DE_trigger is
    signal Q_int: STD_LOGIC;
begin
    proc: process(CLK)
    begin
        if rising_edge(CLK) then
            if CE = '1' then
                Q_int <= D;
            end if;
        end if;
    end process;
    
    Q <= Q_int;
    nQ <= not Q_int;
end Behavioral;
