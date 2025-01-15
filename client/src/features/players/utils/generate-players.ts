import config from "@/config/config";
import { callCreatePlayer } from "@/features/players/api/create-player";
import { PlayerStatus } from "@/types/player";
import { useToast, UseToastOptions } from "@chakra-ui/react";
import { useTranslation } from "react-i18next";

const dict = [
    'leo',
    'rick',
    'charly',
    'nick',
    'bertrand',
    'jules',
    'alex',
    'chris',
    'dima',
    'vova',
    'matt',
    'franck',
    'eva',
    'lena',
    'laura',
    'gwen',
    'alexia',
    'clara',
];

const randomN = (n: number) => {
    return Math.floor(Math.random() * (n + 1));
};

export const generatePlayers = async () => {
    const limit = config.SIMULATE_PLAYERS_NUM;
    const toast = useToast();
    const { t } = useTranslation();
    let count = 0;
    const loadingToastProps = {
        status: 'loading',
        isClosable: false,
        duration: 30000,
    } as UseToastOptions;
    const loadingToast = toast({
        title: t('Message.GeneratingPlayers'),
        description: t('Message.GeneratingCount', { count: count }),
        ...loadingToastProps,
    });
    for (let i = 0; i < limit; i++) {
        const uname =
            dict[randomN(dict.length - 1)] +
            '_' +
            dict[randomN(dict.length - 1)] +
            '_' +
            dict[randomN(dict.length - 1)] +
            '_' +
            randomN(100000);
        const json = {
            Username: uname,
            Status: PlayerStatus.Active,
        };
        callCreatePlayer(json);
        count++;
        if (count % 2 === 0)
            toast.update(loadingToast, {
                description: t('Message.GeneratingCount', { count: count }),
                ...loadingToastProps,
            });
    }
    toast.close(loadingToast);
    toast({
        title: t('Message.GeneratingSuccess', { count: count }),
        status: 'success',
    });
    window.location.reload();
};