library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_fpga is
    port(
        CLK: in std_logic;                         
        LED: out std_logic_vector(3 downto 0)     
    );
end lfsr_fpga;

architecture Behavioral of lfsr_fpga is
    constant N: integer := 4;                   
    constant CLK_FREQ: integer := 100_000_000;
    constant Polynom: std_logic_vector(N-1 downto 0) := "1001";
    
    component lfsr_ext is
        generic(
            N: integer; 
            Polynom: std_logic_vector
        );
        port(
            CLK, RST: in std_logic; 
            Q: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal RST: std_logic := '0';
    signal tick_1hz: std_logic := '0';
    signal clk_counter: integer range 0 to CLK_FREQ-1 := 0;
    signal rst_counter: integer range 0 to 1000 := 0;
    signal lfsr_out: std_logic_vector(N-1 downto 0);
begin
    reset_process: process(CLK)
    begin
        if rising_edge(CLK) then
            if rst_counter < 100 then
                RST <= '1';
                rst_counter <= rst_counter + 1;
            else
                RST <= '0';
            end if;
        end if;
    end process;
    
    freq_divider: process(CLK)
    begin
        if rising_edge(CLK) then
            if clk_counter >= (CLK_FREQ / 2 - 1) then
                tick_1hz <= not tick_1hz;
                clk_counter <= 0;
            else
                clk_counter <= clk_counter + 1;
            end if;
        end if;
    end process;

    lfsr_inst: lfsr_ext
        generic map(
            N => N,
            Polynom => Polynom
        )
        port map(
            CLK => tick_1hz,
            RST => RST,
            Q => lfsr_out
        );
    
    LED <= lfsr_out;
end Behavioral;
