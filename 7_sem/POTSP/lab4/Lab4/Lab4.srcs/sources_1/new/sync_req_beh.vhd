library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity sync_reg_beh is
    generic (
        N: integer
    );
    Port ( 
        CLK, EN: in std_logic;
        Din: in std_logic_vector(N-1 downto 0);
        Dout: out std_logic_vector(N-1 downto 0)
    );
end sync_reg_beh;

architecture Behavioral of sync_reg_beh is
    signal internal_reg: std_logic_vector(N-1 downto 0) := (others => '0');
begin
    proc: process(CLK, EN)
    begin
        if rising_edge(CLK) then
            if EN = '1' then
                internal_reg <= Din;
            end if;
        end if;
    end process;

    Dout <= internal_reg;

end Behavioral;
