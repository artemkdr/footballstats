import { Team } from '@/types/team';
import { Select, SelectProps } from '@chakra-ui/react';

interface SelectTeamProps extends SelectProps {
    teams: Team[];
}

export const SelectTeam: React.FC<SelectTeamProps> = (props) => {
    const { teams, ...rest } = props;
    return (
        <Select {...rest}>
            {teams.map((item) => (
                <option key={item.Id} value={item.Id}>
                    {item.Name}
                </option>
            ))}
        </Select>
    );
};
