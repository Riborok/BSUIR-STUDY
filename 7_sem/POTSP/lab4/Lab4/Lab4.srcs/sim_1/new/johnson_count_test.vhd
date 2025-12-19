library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity johnson_count_test is
end johnson_count_test;

architecture Behavioral of johnson_count_test is
    constant N : integer := 4;
    
    component johnson_count is
        generic (N: integer := N);
        port (
            CLK, RST: in std_logic;
            Q: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic := '0';
    signal Q: std_logic_vector(N-1 downto 0);
begin
    uut: johnson_count
        generic map (N => N)
        port map (
            CLK => CLK,
            RST => RST,
            Q => Q
        );
    
    clk_proc : process
    begin
        CLK <= '0'; wait for 5 ns;
        CLK <= '1'; wait for 5 ns;
    end process;
    
    stimulus_process : process
    begin
        RST <= '1';
        wait for 15 ns;
        RST <= '0';
        wait for 5 ns;
        
        wait for N*20 ns;
    end process stimulus_process;
end Behavioral;
