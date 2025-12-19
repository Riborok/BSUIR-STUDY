library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity johnson_count is
    generic (N : integer := 4);
    port (
        CLK, RST: in std_logic;                      
        Q: out std_logic_vector(N-1 downto 0)
    );
end johnson_count;

architecture Behavioral of johnson_count is
    signal internal_reg: std_logic_vector(N-1 downto 0) := (others => '0');
begin
    proc: process(CLK, RST)
    begin
        if RST = '1' then
            internal_reg <= (others => '0');
        elsif rising_edge(CLK) then
            internal_reg <= (not internal_reg(0)) & internal_reg(N-1 downto 1);
        end if;
    end process;
    
    Q <= internal_reg;
end Behavioral;
