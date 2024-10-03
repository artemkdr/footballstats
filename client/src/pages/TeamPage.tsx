import { Heading, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { convertToTeam, Team } from '../models/Team';

export const TeamPage: FunctionComponent = (): ReactElement => {
	const data : any = useLoaderData();
	const [team, setTeam] = useState<Team>({} as Team);
	const { t } = useTranslation();	

	useEffect(() => {
		setTeam(convertToTeam(data));		
	}, [data]);

	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Team")} {team.Name}</Heading>			
		</VStack>
	)
}