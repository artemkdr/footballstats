import { Table, TableContainer, TableContainerProps, Tbody, Td, Th, Thead, Tr } from "@chakra-ui/react";
import { t } from "i18next";
import { TeamStat } from "../models/TeamStat";
import { CustomLink } from "./CustomLink";

interface StatsTableProps extends TableContainerProps {    
    stats: TeamStat[];
}

export const StatsTable: React.FC<StatsTableProps> = (props) => {
    const { stats, ...rest } = props;
    return (
        <TableContainer {...rest}>
            <Table variant={"simple"} size={["sm", "sm"]} width={"100%"}>
                <Thead>
                    <Tr>
                        <Th>{t("Dashboard.Team")}</Th>
                        <Th>{t("Dashboard.Games")}</Th>
                        <Th>{t("Dashboard.Wins")}</Th>
                        <Th>{t("Dashboard.Losses")}</Th>
                        <Th>{t("Dashboard.WinRatio")}</Th>
                        <Th>{t("Dashboard.GF")}</Th>
                        <Th>{t("Dashboard.GA")}</Th>
                        <Th>{t("Dashboard.GD")}</Th>
                    </Tr>
                </Thead>
                <Tbody>
                    {stats.map((item, index) => (
                        <Tr data-testid="table-stats-row" key={index} width={"100%"}>
                            <Td>                                
                                <CustomLink link={`/team/${item.Id}`} text={item.Name} />
                            </Td>
                            <Td>{item.Games}</Td>
                            <Td>{item.Wins}</Td>
                            <Td>{item.Losses}</Td>
                            <Td>{Math.round(100 * item.Wins / item.Games)/100}</Td>
                            <Td>{item.GF}</Td>
                            <Td>{item.GA}</Td>
                            <Td>{item.GF - item.GA}</Td>
                        </Tr>                            
                    ))}
                </Tbody>
            </Table>
        </TableContainer>
    )
}