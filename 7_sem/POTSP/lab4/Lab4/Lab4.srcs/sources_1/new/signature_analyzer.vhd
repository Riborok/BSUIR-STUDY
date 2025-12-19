library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity signature_analyzer is
    generic (
        N: integer;
        Polynom: std_logic_vector
    );
    port (
        CLK, RST, Din: in std_logic;
        Sign: out std_logic_vector(N-1 downto 0)
    );
end signature_analyzer;

architecture Behavioral of signature_analyzer is
    signal reg: std_logic_vector(N-1 downto 0) := (others => '0');
    signal feedback: std_logic;
begin

    feedback <= reg(N-1);
    process(CLK, RST)
        begin
            if RST = '1' then
                reg <= (others => '0');
            elsif rising_edge(CLK) then
                for i in N-1 downto 1 loop
                    if Polynom(i) = '1' then
                        reg(i) <= reg(i-1) xor feedback;
                    else
                        reg(i) <= reg(i-1);
                    end if;
                end loop;
                
                if Polynom(0) = '1' then
                    reg(0) <= Din xor feedback;
                else
                    reg(0) <= Din;
                end if;
            end if;
        end process;
    
    Sign <= reg;
end Behavioral;
