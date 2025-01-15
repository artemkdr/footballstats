import { Player } from '@/types/player';
import { Select, SelectProps } from '@chakra-ui/react';

interface SelectPlayerProps extends SelectProps {
    players: Player[];
}

export const SelectPlayer: React.FC<SelectPlayerProps> = (props) => {
    const { players, ...rest } = props;
    return (
        <Select {...rest}>
            {players.map((item) => (
                <option key={item.Username} value={item.Username}>
                    {item.Username}
                </option>
            ))}
        </Select>
    );
};
