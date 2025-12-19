library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

entity rom_counter is
    generic (
        N      : integer := 4;
        DEPTH  : integer := 8
    );
    port (
        CLK, RST : in  std_logic;
        Q        : out std_logic_vector(N-1 downto 0)
    );
end rom_counter;

architecture Behavioral of rom_counter is
    type rom_type is array (0 to DEPTH-1) of std_logic_vector(N-1 downto 0);

    constant ROM : rom_type := (
        0 => "0001",
        1 => "0011",
        2 => "0010",
        3 => "0110",
        4 => "0100",
        5 => "1100",
        6 => "1000",
        7 => "1001"
    );

    signal addr : integer range 0 to DEPTH-1 := 0;

begin

    process (CLK, RST)
    begin
        if RST = '1' then
            addr <= 0;
        elsif rising_edge(CLK) then
            if addr = DEPTH-1 then
                addr <= 0;
            else
                addr <= addr + 1;
            end if;
        end if;
    end process;

    Q <= ROM(addr);

end Behavioral;
