library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity d_trigger_en is
    Port ( 
        CLK, EN, Din: in std_logic;
        Dout: out std_logic
    );
end d_trigger_en;

architecture Behavioral of d_trigger_en is

begin
    proc: process(CLK, EN)
    begin
        if rising_edge(CLK) then
            if EN = '1' then
                Dout <= Din;
            end if;
        end if;
    end process;
end Behavioral;
