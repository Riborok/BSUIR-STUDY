library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_trigger is
    Port (
        S, R, CLK: in STD_LOGIC;
        Q, nQ: out STD_LOGIC
    );
end RS_trigger;

architecture Behavioral of RS_trigger is
    signal Q_int: STD_LOGIC;
begin
    proc: process(CLK)
    begin
        if rising_edge(CLK) then
            if S = '1' and R = '0' then
                Q_int <= '1';
            elsif S = '0' and R = '1' then
                Q_int <= '0';
            elsif S = '1' and R = '1' then
                 Q_int <= 'X'; 
            end if;
        end if;
    end process;

    Q  <= Q_int;
    nQ <= not Q_int;
    
end Behavioral;
