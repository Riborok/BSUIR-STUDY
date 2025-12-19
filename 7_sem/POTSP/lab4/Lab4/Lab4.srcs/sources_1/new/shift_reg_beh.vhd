library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity shift_reg_beh is
    generic (
        N: integer
    );
    port (
        CLK, RST, SE, Sin: in std_logic;                          
        Pout: out std_logic_vector(N-1 downto 0)
    );
end shift_reg_beh;

architecture Behavioral of shift_reg_beh is
    signal internal_reg : std_logic_vector(N-1 downto 0);
begin
    proc: process(CLK, RST)
    begin
        if RST = '1' then
            internal_reg <= (others => '0');
            
        elsif rising_edge(CLK) then
            if SE = '1' then
                internal_reg <= Sin & internal_reg(N-1 downto 1);
            end if;
        end if;
    end process;
    
    Pout <= internal_reg;
end Behavioral;
