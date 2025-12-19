library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity JK_trigger is
    Port (
        J, K, CLK: in  STD_LOGIC;
        Q, nQ: out STD_LOGIC
    );
end JK_trigger;

architecture Behavioral of JK_trigger is
    signal Q_int: STD_LOGIC;
begin
    proc: process(CLK)
    begin 
        if rising_edge(CLK) then
            if J = '0' and K = '0' then
                null;
            elsif J = '0' and K = '1' then
                Q_int <= '0';
            elsif J = '1' and K = '0' then
                Q_int <= '1';
            elsif J = '1' and K = '1' then
                Q_int <= not Q_int;
            end if;
        end if;
    end process;
    
    Q  <= Q_int;
    nQ <= not Q_int;
end Behavioral;
