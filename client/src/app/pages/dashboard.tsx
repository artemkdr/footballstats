import { SimpleSuspense } from '@/components/simple-suspense';
import { StatsTable } from '@/features/stats/stats-table';
import {    
    TeamStat,
} from '@/features/stats/types/team-stat';
import { Heading, VStack } from '@chakra-ui/react';
import { ReactElement, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useLoaderData } from 'react-router-dom';

export const Dashboard = (): ReactElement => {
    const data = useLoaderData() as TeamStat[];
    const { t } = useTranslation();
    const [teamStats, setTeamStats] = useState<TeamStat[] | undefined>(undefined);

    useEffect(() => {
        setTeamStats(data);
    }, [data]);

    return (
        <VStack align={'left'} spacing={5}>
            <Heading as="h2" size="md">
                {t('Dashboard.Title')}
            </Heading>
            <SimpleSuspense fallback={t('Loading')} emptyText={t('Empty')}>
                {teamStats && <StatsTable maxWidth={['100%', 1000]} stats={teamStats} />}
            </SimpleSuspense>
        </VStack>
    );
};
