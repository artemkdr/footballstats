import config from "@/config/config";
import { callCreateGame } from "@/features/games/api/create-game";
import { GameStatus } from "@/types/game";
import { Team } from "@/types/team";
import { useToast, UseToastOptions } from "@chakra-ui/react";
import { useTranslation } from "react-i18next";

export const simulateGames = async (teams : Team[]) => {
    const limit = config.SIMULATE_GAMES_LIMIT;
    const toast = useToast();
    const { t } = useTranslation();
    let count = 0;
    const loadingToastProps = {
        status: 'loading',
        isClosable: false,
        duration: 300000,
    } as UseToastOptions;
    const loadingToast = toast({
        title: t('Message.SimulatingGames'),
        description: t('Message.SimulatingCount', { count: count }),
        ...loadingToastProps,
    });
    for (let i = 0; i < teams.length; i++) {
        const team1 = teams[i];
        for (let j = 0; j < teams.length; j++) {
            if (i === j) continue;
            if (count > limit) break;

            const team2 = teams[j];
            const json = {
                Goals1: Math.floor(Math.random() * 11),
                Goals2: Math.floor(Math.random() * 11),
                Team1: team1.Id,
                Team2: team2.Id,
                Status: GameStatus.Completed,
                CompleteDate: new Date(),
            };
            await callCreateGame(json);
            count++;
            if (count % 10 === 0)
                toast.update(loadingToast, {
                    description: t('Message.SimulatingCount', {
                        count: count,
                    }),
                    ...loadingToastProps,
                });
        }
        if (count > limit) break;
    }
    toast.close(loadingToast);
    toast({
        title: t('Message.SimulateSuccess', { count: count }),
        status: 'success',
    });
    window.location.reload();
};