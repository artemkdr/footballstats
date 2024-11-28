import { StatsTable } from '@/features/stats/stats-table';
import {
    convertDataToTeamStatList,
    TeamStat,
} from '@/features/stats/types/team-stat';
import { Heading, VStack } from '@chakra-ui/react';
import { FunctionComponent, ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';

export const Dashboard: FunctionComponent = (): ReactElement => {
    const data: any = useLoaderData();
    const { t } = useTranslation();
    const [teamStats, setTeamStats] = useState<TeamStat[]>([]);

    useEffect(() => {
        setTeamStats(convertDataToTeamStatList(data));
    }, [data]);

    return (
        <VStack align={'left'} spacing={5}>
            <Heading as="h2" size="md">
                {t('Dashboard.Title')}
            </Heading>

            <StatsTable maxWidth={['100%', 1000]} stats={teamStats} />
        </VStack>
    );
};
