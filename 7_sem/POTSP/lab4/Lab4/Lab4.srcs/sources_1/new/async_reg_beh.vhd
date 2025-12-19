library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity async_reg_beh is
    generic (
        N: integer
    );
    Port ( 
        EN: in std_logic;
        Din: in std_logic_vector(N-1 downto 0);
        Dout: out std_logic_vector(N-1 downto 0)
    );
end async_reg_beh;

architecture Behavioral of async_reg_beh is

begin
    proc: process(EN, Din)
    begin
        if EN = '1' then
            Dout <= Din;
        end if;
    end process;
end Behavioral;
