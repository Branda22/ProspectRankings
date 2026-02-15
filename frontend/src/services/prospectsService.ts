import api from './api';

export interface ProspectInput {
  player_name: string;
  team: string;
  position: string;
  age: number;
  eta: string;
  rank: number;
}

export async function uploadProspects(
  source: string,
  list: ProspectInput[]
): Promise<string[]> {
  const response = await api.post<string[]>('/prospects', { source, list });
  return response.data;
}

export async function calculateRankings(): Promise<void> {
  await api.post('/prospects/calculate');
}
