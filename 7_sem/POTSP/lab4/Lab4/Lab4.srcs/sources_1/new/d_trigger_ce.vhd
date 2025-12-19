library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity d_trigger_ce is
    port (
        CLK, CLR, CE, D: in std_logic;  
        Dout: out std_logic
    );
end d_trigger_ce;

architecture Behavioral of d_trigger_ce is

begin
    proc: process(CLK, CLR, CE)
    begin
        if CLR = '1' then
            Dout <= '0';
        elsif rising_edge(CLK) then
            if CE = '1' then
                Dout <= D;
            end if;
        end if;
    end process;
end Behavioral;
