import {
	HStack,
	Heading,
	VStack
} from '@chakra-ui/react';
import { FunctionComponent, ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';
import { CustomLink } from '../components/CustomLink';
import { convertDataToTeamList } from '../models/Team';

export const Teams: FunctionComponent = (): ReactElement => {
	const { t } = useTranslation();	
	const data = useLoaderData();	
	const teamsList = convertDataToTeamList(data);
	
	return (
		<VStack spacing={5} align="left">
			<Heading as="h2" size="md">{t("Teams.Title")}</Heading>
			<VStack spacing={5} align="left" paddingLeft={3}>
				{teamsList?.map((item, index) => (					
					<HStack spacing={2} key={index}>						
						<CustomLink link={`/team/${item.Id}`} text={item.Name} />
					</HStack>								
				))}
			</VStack>
		</VStack>
	)
}