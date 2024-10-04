import { Link as ChakraLink, Heading, Table, TableContainer, Tbody, Td, Text, Th, Thead, Tr, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link as ReactRouterLink, useLoaderData } from 'react-router-dom';
import { convertDataToTeamStatList, TeamStat } from '../models/TeamStat';

export const Dashboard: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();        
    const { t } = useTranslation();
	const [teamStats, setTeamStats] = useState<TeamStat[]>([]);

	useEffect(() => {      
        setTeamStats(convertDataToTeamStatList(data));    
    }, [data]);

	return (
		<VStack align={"left"} spacing={5}>
            <Heading as="h2" size="md">{t("Dashboard.Title")}</Heading>

			<TableContainer maxWidth={["100%", 1000]}>
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
                        {teamStats.map((item, index) => (
                            <Tr data-testid="table-stats-row" key={index} width={"100%"}>
                                <Td>
								<ChakraLink as={ReactRouterLink} to={`/team/${item.Id}`}>														
									<Text>{item.Name}</Text>																
								</ChakraLink>
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
		</VStack>
	)
}