import { toInt } from '@/lib/utils/converters';
import { convertToTeam, Team, teamHasPlayer } from '@/types/team';

export enum GameStatus {
    NotStarted = 'NotStarted',
    Playing = 'Playing',
    Completed = 'Completed',
    Cancelled = 'Cancelled',
}

export interface GetGameResponse {
    id: number;
    team1: unknown;
    team1Detail: unknown;    
    team2: unknown;
    team2Detail: unknown;
    goals1: number;
    goals2: number;
    status: string;
    completeDate: string;
    createDate: string;
    modifyDate: string;
    vars: unknown;
}

export interface CreateGameResponse {
    id: number;
}
export interface UpdateGameResponse {
    id: number;
}
export interface Game {
    Id: number;
    Team1: Team;
    Team2: Team;
    Goals1: number;
    Goals2: number;
    Vars: unknown;
    CreateDate: Date;
    ModifyDate: Date;
    CompleteDate: Date;
    Status: GameStatus;
}

export const convertToGame = <T = Game>(json: unknown): T => {
    const data = json as GetGameResponse;
    return {
        Id: toInt(data?.id),
        Team1: data.team1 ? convertToTeam(data.team1Detail ?? data.team1) : {} as Team,
        Team2: data.team2 ? convertToTeam(data.team2Detail ?? data.team2) : {} as Team,
        Goals1: toInt(data?.goals1),
        Goals2: toInt(data?.goals2),
        Vars: data.vars,
        CreateDate: new Date(data.createDate),
        ModifyDate: new Date(data.modifyDate),
        CompleteDate: new Date(data.completeDate),
        Status: data.status as GameStatus,
    } as T;
};

export const convertToGameList = (listData: unknown[]) => {
    const list = [] as Game[];
    if (listData instanceof Array) {
        for (const ol of listData) {
            const o = convertToGame(ol);
            if (o != null && o.Id != null) {
                list.push(o);
            }
        }
    }
    return list;
};

export const getGameStatusColor = (status: GameStatus): string => {
    switch (status) {
        case GameStatus.NotStarted:
            return 'yellow';
        case GameStatus.Playing:
            return 'green';
        case GameStatus.Completed:
            return 'blue';
        case GameStatus.Cancelled:
            return 'gray';
    }
    return '';
};

export enum GameResult {
    Win = 'Win',
    Loss = 'Loss',
    Draw = 'Draw',
    None = 'None',
}

export const getGameColorForResult = (result: GameResult): string => {
    switch (result) {
        case GameResult.Win:
            return 'green';
        case GameResult.Loss:
            return 'red';
    }
    return 'gray';
};

export const getGameResultFor = (game: Game, teamId: number): GameResult => {
    if (game.Status === GameStatus.Completed) {
        if (
            (game.Team1?.Id === teamId && game.Goals1 > game.Goals2) ||
            (game.Team2?.Id === teamId && game.Goals2 > game.Goals1)
        )
            return GameResult.Win;

        if (
            (game.Team1?.Id === teamId && game.Goals1 < game.Goals2) ||
            (game.Team2?.Id === teamId && game.Goals2 < game.Goals1)
        )
            return GameResult.Loss;

        return GameResult.Draw;
    }
    return GameResult.None;
};

export const getGameResultForPlayer = (
    game: Game,
    username: string
): GameResult => {
    if (game.Status === GameStatus.Completed) {
        if (
            (teamHasPlayer(game.Team1, username) &&
                game.Goals1 > game.Goals2) ||
            (teamHasPlayer(game.Team2, username) && game.Goals2 > game.Goals1)
        )
            return GameResult.Win;

        if (
            (teamHasPlayer(game.Team1, username) &&
                game.Goals1 < game.Goals2) ||
            (teamHasPlayer(game.Team2, username) && game.Goals2 < game.Goals1)
        )
            return GameResult.Loss;

        return GameResult.Draw;
    }
    return GameResult.None;
};

export const isValidGame = (game: Game | undefined | null) => {
    if (
        game == null ||
        game.Id == null ||
        game.Team1 == null ||
        game.Team2 == null ||
        game.Team1?.Id === game.Team2?.Id
    )
        return false;
    return true;
};
