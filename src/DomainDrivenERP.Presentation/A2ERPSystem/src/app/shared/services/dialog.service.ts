import { Injectable } from '@angular/core';
import { TuiDialogService, TuiDialogOptions } from '@taiga-ui/core';
import { PolymorpheusComponent } from '@tinkoff/ng-polymorpheus';
import { Observable, firstValueFrom, from } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { 
  ConfirmationDialogComponent, 
  ConfirmationDialogData 
} from '../components/confirmation-dialog/confirmation-dialog.component';

export interface DialogConfig extends ConfirmationDialogData {
  title?: string;
  size?: 's' | 'm' | 'l';
}

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  constructor(private tuiDialogService: TuiDialogService) {}

  confirm(config: DialogConfig): Observable<boolean> {
    const dialogData: Required<ConfirmationDialogData> = {
      message: config.message,
      status: config.status || 'info',
      confirmText: config.confirmText || 'Confirm',
      cancelText: config.cancelText || 'Cancel'
    };

    const options: Partial<TuiDialogOptions<Required<ConfirmationDialogData>>> = {
      label: config.title || 'Confirm Action',
      size: config.size || 's',
      data: dialogData,
      closeable: true,
      dismissible: true
    };

    return this.tuiDialogService.open<boolean>(
      new PolymorpheusComponent(ConfirmationDialogComponent),
      options
    ).pipe(
      map(result => result ?? false),
      catchError(() => from([false]))
    );
  }

  async confirmDeletion(itemName: string): Promise<boolean> {
    try {
      return await firstValueFrom(this.confirm({
        title: 'Confirm Deletion',
        message: `Are you sure you want to delete "${itemName}"? This action cannot be undone.`,
        confirmText: 'Delete',
        cancelText: 'Keep',
        status: 'error',
        size: 's'
      }));
    } catch {
      return false;
    }
  }

  async confirmDiscard(itemType: string): Promise<boolean> {
    try {
      return await firstValueFrom(this.confirm({
        title: 'Discard Changes',
        message: `You have unsaved changes to this ${itemType}. Do you want to discard them?`,
        confirmText: 'Discard',
        cancelText: 'Keep Editing',
        status: 'warning',
        size: 's'
      }));
    } catch {
      return false;
    }
  }

  async confirmNavigation(): Promise<boolean> {
    try {
      return await firstValueFrom(this.confirm({
        title: 'Unsaved Changes',
        message: 'You have unsaved changes. Do you want to leave this page?',
        confirmText: 'Leave',
        cancelText: 'Stay',
        status: 'warning',
        size: 's'
      }));
    } catch {
      return false;
    }
  }

  showSuccess(message: string): Observable<void> {
    return this.confirm({
      title: 'Success',
      message,
      status: 'success',
      confirmText: 'OK',
      cancelText: '',
      size: 's'
    }).pipe(map(() => void 0));
  }

  showError(message: string): Observable<void> {
    return this.confirm({
      title: 'Error',
      message,
      status: 'error',
      confirmText: 'OK',
      cancelText: '',
      size: 's'
    }).pipe(map(() => void 0));
  }

  showWarning(message: string): Observable<void> {
    return this.confirm({
      title: 'Warning',
      message,
      status: 'warning',
      confirmText: 'OK',
      cancelText: '',
      size: 's'
    }).pipe(map(() => void 0));
  }

  showInfo(message: string): Observable<void> {
    return this.confirm({
      title: 'Information',
      message,
      status: 'info',
      confirmText: 'OK',
      cancelText: '',
      size: 's'
    }).pipe(map(() => void 0));
  }
}