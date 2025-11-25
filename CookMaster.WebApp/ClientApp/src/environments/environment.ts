import {IAppSettings} from '../app/shared/interfaces/iapp-settings';

export const environment = {
  production: false,
  appSettings: <IAppSettings>({
    serverUrl: 'http://localhost:11001'
  })
};
